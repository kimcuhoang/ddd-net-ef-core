using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly IFixture Fixture;
        private readonly IHost _host;

        public SharedFixture()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json");
                });

            this._host = hostBuilder.Start();
            this._serviceScopeFactory = this._host.Services.GetService<IServiceScopeFactory>();

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };

            this.Fixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            await this.ResetCheckpoint();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        #endregion

        public async Task RepositoryExecute<TAggregate, TIdentity>(Func<IRepository<TAggregate, TIdentity>, Task> action) where TAggregate : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            using var scope = this._serviceScopeFactory.CreateScope();
            using var repositoryFactory = scope.ServiceProvider.GetService<IRepositoryFactory>();
            var repository = repositoryFactory.CreateRepository<TAggregate, TIdentity>();
            await action(repository);
        }

        public async Task SeedingData<TAggregate, TIdentity>(params TAggregate[] entities) where TAggregate : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            if (entities != null && entities.ToArray().Any())
            {
                using var scope = this._serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<DbContext>();
                await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                try
                {
                    await dbContext.Set<TAggregate>().AddRangeAsync(entities);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task ResetCheckpoint()
        {
            using var serviceScope = this._serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<DbContext>();

            var dbConnection = dbContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();
            await this._checkpoint.Reset(dbConnection);
        }
    }
}
