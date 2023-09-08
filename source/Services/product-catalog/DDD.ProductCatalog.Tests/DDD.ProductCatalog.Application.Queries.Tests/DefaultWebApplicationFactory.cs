using DNK.DDD.IntegrationTests;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DDD.ProductCatalog.Application.Queries.Tests;

[CollectionDefinition(nameof(QueriesTestCollection))]
public class QueriesTestCollection : ICollectionFixture<DefaultWebApplicationFactory>
{
}

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
{
    
}
