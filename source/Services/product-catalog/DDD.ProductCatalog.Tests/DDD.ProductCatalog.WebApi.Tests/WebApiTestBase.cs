using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.WebApi.Tests;

[Collection(nameof(WebApiTestCollection))]
public abstract class WebApiTestBase(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : IntegrationTestBase<WebApiTestCollectionFixture, DefaultWebApplicationFactory, Program>(testCollectionFixture, output)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
    }
}
