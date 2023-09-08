using DNK.DDD.IntegrationTests;


namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

[Collection(nameof(EfCoreTestCollection))]
public abstract class TestEfCoreBase : IntegrationTestBase<TestEfCoreFixture, DefaultWebApplicationFactory, Program>
{
    protected TestEfCoreBase(TestEfCoreFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }
}
