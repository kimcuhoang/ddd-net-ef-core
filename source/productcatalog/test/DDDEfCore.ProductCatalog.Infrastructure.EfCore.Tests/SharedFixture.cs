using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private static readonly AsyncLock Mutex = new AsyncLock();

        private static bool _initialized;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SharedFixture()
        {
            var host = new Mock<IHostingEnvironment>();
            host.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            var services = new ServiceCollection();
            var startup = new TestStartup(host.Object);
            startup.ConfigureServices(services);

            this._serviceScopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            if (_initialized)
                return;

            using (await Mutex.LockAsync())
            {
                if (_initialized)
                    return;

                await this.ResetCheckpoint();

                _initialized = true;
            }
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        #endregion

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = this._serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<DbContext>();
                using (var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        await action(scope.ServiceProvider);
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task SeedingData<T>(params T[] entities) where T : AggregateRoot
        {
            if (entities != null && entities.Any())
            {
                await this.ExecuteScopeAsync(async services =>
                {
                    var dbContext = services.GetService<DbContext>();
                    await dbContext.Set<T>().AddRangeAsync(entities);
                    await dbContext.SaveChangesAsync();
                });
            }
        }

        private async Task ResetCheckpoint()
        {
            var checkPoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };

            using (var serviceScope = this._serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<DbContext>();
                var databaseMigration = serviceScope.ServiceProvider.GetService<DatabaseMigration>();
                await databaseMigration.ApplyMigration();

                
                var dbConnection = dbContext.Database.GetDbConnection();
                await dbConnection.OpenAsync();
                await checkPoint.Reset(dbConnection);
            }
        }
    }
}
