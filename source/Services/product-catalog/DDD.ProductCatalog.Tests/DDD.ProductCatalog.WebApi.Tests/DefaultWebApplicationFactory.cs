using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.WebApi.Tests;

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
{
    public DefaultWebApplicationFactory(string connectionString) : base(connectionString)
    {
    }

    protected override Dictionary<string, string?> InMemorySettings
    {
        get
        {
            var inMemorySettings = base.InMemorySettings;

            return inMemorySettings;
        }
    }
}
