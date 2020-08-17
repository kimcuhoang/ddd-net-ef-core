using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        protected const string TestEnvironment = "Test";
        private readonly Checkpoint _checkpoint;

        public IHost Host { get; private set; }
        public IFixture Fixture { get; }

        public SharedFixture()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", TestEnvironment);

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
            this.Fixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            var hostBuilder = Program.CreateHostBuilder(new string[0]).UseEnvironment(TestEnvironment)
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost.UseTestServer();
                });

            this.Host = await hostBuilder.StartAsync();
        }

        public virtual async Task DisposeAsync()
        {
            var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
            var dbConnection = dbContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();

            await this._checkpoint.Reset(dbConnection);
            await this.Host.StopAsync();
        }

        #endregion

        public async Task RepositoryExecute<TAggregate, TIdentity>(Func<IRepository<TAggregate, TIdentity>, Task> action) where TAggregate : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();
            using var repositoryFactory = scope.ServiceProvider.GetService<IRepositoryFactory>();
            var repository = repositoryFactory.CreateRepository<TAggregate, TIdentity>();
            await action(repository);
        }

        public async Task SeedingData<TAggregate, TIdentity>(params TAggregate[] entities) where TAggregate : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            if (entities != null && entities.ToArray().Any())
            {
                var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
                using var scope = serviceScopeFactory.CreateScope();
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
    }
}
