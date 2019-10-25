using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests
{
    [CollectionDefinition(nameof(SharedFixture))]
    public class SharedFixtureCollection : ICollectionFixture<SharedFixture>
    {
        // ... no members required ... 
    }
}
