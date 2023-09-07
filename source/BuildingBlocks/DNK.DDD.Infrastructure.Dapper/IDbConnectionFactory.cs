using System.Data;

namespace DNK.DDD.Infrastructure.Dapper;
public interface IDbConnectionFactory: IDisposable
{
    Task<IDbConnection> GetConnection(CancellationToken cancellationToken = default);

    IDbConnection GetConnection();
}
