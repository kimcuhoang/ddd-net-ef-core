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
using AutoFixture;
using DDDEfCore.Core.Common;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected readonly IFixture Fixture;

        public SharedFixture()
        {
            var host = new Mock<IWebHostEnvironment>();
            host.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            var services = new ServiceCollection();
            var startup = new TestStartup(host.Object);
            startup.ConfigureServices(services);

            this._serviceScopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            this.Fixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            await this.ResetCheckpoint();
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

        public async Task RepositoryExecute<TAggregate>(Func<IRepository<TAggregate>, Task> action) where TAggregate : AggregateRoot
        {
            using (var scope = this._serviceScopeFactory.CreateScope())
            {
                var repositoryFactory = scope.ServiceProvider.GetService<IRepositoryFactory>();
                using (var repository = repositoryFactory.CreateRepository<TAggregate>())
                {
                    await action(repository);
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
