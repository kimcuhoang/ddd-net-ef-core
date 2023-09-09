using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
{
    public DefaultWebApplicationFactory(string connectionString) : base(connectionString)
    {
    }
}
