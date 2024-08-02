using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

public class DefaultWebApplicationFactory(string connectionString) : WebApplicationFactoryBase<Program>(connectionString)
{
}
