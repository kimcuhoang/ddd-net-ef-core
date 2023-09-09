using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.WebApi.Tests;

[Collection(nameof(WebApiTestCollection))]
public abstract class WebApiTestBase : IntegrationTestBase<WebApiTestCollectionFixture, DefaultWebApplicationFactory, Program>
{
    protected WebApiTestBase(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) 
        : base(testCollectionFixture, output)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
    }
}
