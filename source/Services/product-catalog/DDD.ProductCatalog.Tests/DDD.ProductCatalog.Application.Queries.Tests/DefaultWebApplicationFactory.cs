using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.Application.Queries.Tests;

public class DefaultWebApplicationFactory(string connectionString) : WebApplicationFactoryBase<Program>(connectionString)
{
}
