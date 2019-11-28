using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DDDEfCore.ProductCatalog.WebApi.Tests
{
    [CollectionDefinition(nameof(SharedFixture))]
    public class SharedFixtureCollection : ICollectionFixture<SharedFixture>
    {
        // ... no members required ... 
    }
}
