using DNK.DDD.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace DDD.ProductCatalog.WebApi.Tests;

[CollectionDefinition(nameof(WebApiTestCollection))]
public class WebApiTestCollection : ICollectionFixture<WebApiTestCollectionFixture>
{

}

public class WebApiTestCollectionFixture : TestCollectionFixtureBase<DefaultWebApplicationFactory, Program>
{
    protected override bool AutoMigration => true;
}
