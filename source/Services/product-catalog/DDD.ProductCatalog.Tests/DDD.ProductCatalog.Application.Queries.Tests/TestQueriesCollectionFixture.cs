using DNK.DDD.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace DDD.ProductCatalog.Application.Queries.Tests;


[CollectionDefinition(nameof(QueriesTestCollection))]
public class QueriesTestCollection : ICollectionFixture<TestQueriesCollectionFixture>
{
}

public class TestQueriesCollectionFixture : TestCollectionFixtureBase<DefaultWebApplicationFactory, Program>
{
    protected override async Task ApplyMigrations(DatabaseFacade database)
    {
        await this.Factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            var database = dbContext.Database;
            var pendingMigrations = await database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await database.MigrateAsync();
            }
        });
    }
}
