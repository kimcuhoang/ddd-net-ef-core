using DNK.DDD.IntegrationTests;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DDD.ProductCatalog.WebApi.Tests;

[CollectionDefinition(nameof(WebApiTestCollection))]
public class WebApiTestCollection : ICollectionFixture<DefaultWebApplicationFactory>
{
    
}

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
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
