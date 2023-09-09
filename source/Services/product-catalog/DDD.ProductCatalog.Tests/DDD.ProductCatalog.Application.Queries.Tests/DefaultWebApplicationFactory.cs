using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.Application.Queries.Tests;

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
{
    public DefaultWebApplicationFactory(string connectionString) : base(connectionString)
    {
    }
}
