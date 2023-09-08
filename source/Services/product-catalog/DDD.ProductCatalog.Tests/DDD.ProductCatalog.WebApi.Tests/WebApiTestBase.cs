using DNK.DDD.IntegrationTests;

namespace DDD.ProductCatalog.WebApi.Tests;

[Collection(nameof(WebApiTestCollection))]
public abstract class WebApiTestBase : IntegrationTestBase<WebApiTestFixture, DefaultWebApplicationFactory, Program>
{
    protected WebApiTestBase(WebApiTestFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
    }
}
