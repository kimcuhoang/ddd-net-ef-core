using Xunit;
using DNK.DDD.IntegrationTests;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

[CollectionDefinition(nameof(EfCoreTestCollection))]
public class EfCoreTestCollection : ICollectionFixture<DefaultWebApplicationFactory>
{
}

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
{
    
}
