using System.Data;
using System.Data.SqlClient;

namespace DNK.DDD.Infrastructure.Dapper.MsSqlServer;
public class MsSqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    private IDbConnection _connection;

    public MsSqlServerConnectionFactory(string connectionString)
    {
        this._connectionString = connectionString;
    }

    public void Dispose()
    {
        if (this._connection != null && this._connection.State == ConnectionState.Open)
        {
            this._connection.Dispose();
        }
    }

    public async Task<IDbConnection> GetConnection(CancellationToken cancellationToken = default)
    {
        if (this._connection == null || this._connection.State != ConnectionState.Open)
        {
            this._connection = new SqlConnection(_connectionString);
            await((SqlConnection)this._connection).OpenAsync(cancellationToken);
        }

        return this._connection;
    }

    public IDbConnection GetConnection()
    {
        if (this._connection == null || this._connection.State != ConnectionState.Open)
        {
            this._connection = new SqlConnection(_connectionString);
            this._connection.Open();
        }

        return this._connection;
    }
}
