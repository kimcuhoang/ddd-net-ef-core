using DNK.DDD.IntegrationTests;


namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

[Collection(nameof(EfCoreTestCollection))]
public abstract class TestEfCoreBase(TestEfCoreCollectionFixture testCollectionFixture, ITestOutputHelper output) : IntegrationTestBase<TestEfCoreCollectionFixture, DefaultWebApplicationFactory, Program>(testCollectionFixture, output)
{
}
