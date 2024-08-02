using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.WebApi.Tests;

public class DefaultWebApplicationFactory(string connectionString) : WebApplicationFactoryBase<Program>(connectionString)
{
    protected override Dictionary<string, string?> InMemorySettings
    {
        get
        {
            var inMemorySettings = base.InMemorySettings;

            return inMemorySettings;
        }
    }
}
