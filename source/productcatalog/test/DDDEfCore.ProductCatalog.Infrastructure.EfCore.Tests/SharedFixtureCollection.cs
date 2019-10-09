using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    [CollectionDefinition(nameof(SharedFixture))]
    public class SharedFixtureCollection : ICollectionFixture<SharedFixture>
    {
        // ... no members required ... 
    }
}
