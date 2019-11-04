using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.Db
{
    public sealed class SqlServerDbConnectionFactory : IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public SqlServerDbConnectionFactory(string connectionString)
            => this._connectionString = connectionString;

        public async Task<IDbConnection> GetConnection(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this._connection == null || this._connection.State != ConnectionState.Open)
            {
                this._connection = new SqlConnection(_connectionString);
                await ((SqlConnection) this._connection).OpenAsync(cancellationToken);
            }

            return this._connection;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (this._connection != null && this._connection.State == ConnectionState.Open)
            {
                this._connection.Dispose();
            }
        }

        #endregion
    }
}
