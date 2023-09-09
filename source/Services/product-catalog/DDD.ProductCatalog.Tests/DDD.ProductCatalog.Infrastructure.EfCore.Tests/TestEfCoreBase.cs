using DNK.DDD.IntegrationTests;


namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

[Collection(nameof(EfCoreTestCollection))]
public abstract class TestEfCoreBase : IntegrationTestBase<TestEfCoreCollectionFixture, DefaultWebApplicationFactory, Program>
{
    protected TestEfCoreBase(TestEfCoreCollectionFixture testCollectionFixture, ITestOutputHelper output) 
        : base(testCollectionFixture, output)
    {
    }
}
