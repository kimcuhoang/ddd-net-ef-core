using DDDEfCore.Core.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;

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
            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            }; ;
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            await this.ResetCheckpoint();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        #endregion

        public async Task SeedingData<T>(params T[] entities) where T : AggregateRoot
        {
            if (entities != null && entities.Any())
            {
                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<DbContext>();
                    using (var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            await dbContext.Set<T>().AddRangeAsync(entities);
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
        }

        public async Task ExecuteScopeAsync(Func<SqlServerDbConnectionFactory, Task> action)
        {
            using (var scope = this._serviceScopeFactory.CreateScope())
            {
                var sqlServerDbConnection = scope.ServiceProvider.GetService<SqlServerDbConnectionFactory>();
                await action(sqlServerDbConnection);
            }
        }

        public async Task ExecuteTest(Func<IServiceProvider, Task> testFunc)
        {
            using var scope = this._serviceScopeFactory.CreateScope();
            await testFunc(scope.ServiceProvider);
        }

        private async Task ResetCheckpoint()
        {
            using (var serviceScope = this._serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<DbContext>();

                var databaseMigration = serviceScope.ServiceProvider.GetService<DatabaseMigration>();
                await databaseMigration.ApplyMigration();

                var dbConnection = dbContext.Database.GetDbConnection();
                await dbConnection.OpenAsync();
                await this._checkpoint.Reset(dbConnection);
            }
        }
    }
}
