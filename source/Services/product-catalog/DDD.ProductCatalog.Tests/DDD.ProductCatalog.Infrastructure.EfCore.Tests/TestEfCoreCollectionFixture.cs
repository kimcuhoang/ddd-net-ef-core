using DNK.DDD.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests;

[CollectionDefinition(nameof(EfCoreTestCollection))]
public class EfCoreTestCollection : ICollectionFixture<TestEfCoreCollectionFixture>
{
}

public class TestEfCoreCollectionFixture : TestCollectionFixtureBase<DefaultWebApplicationFactory, Program>
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
