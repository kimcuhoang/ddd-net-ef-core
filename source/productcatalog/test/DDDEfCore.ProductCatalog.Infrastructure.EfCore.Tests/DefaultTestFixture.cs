using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    public class DefaultTestFixture : IAsyncLifetime
    {
        public IFixture Fixture { get; }

        protected readonly DefaultWebApplicationFactory _factory;

        public DefaultTestFixture(DefaultWebApplicationFactory factory)
        {
            this._factory = factory;
            this.Fixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            await this._factory.StartContainerAsync();

            await this.ExecuteDbContextAsync(async dbContext =>
            {
                var database = dbContext.Database;
                var pendingMigrations = await database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await database.MigrateAsync();
                }
            });
        }

        public virtual async Task DisposeAsync() => await Task.Yield();

        public async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
        {
            await this._factory.ExecuteServiceAsync(func);
        }

        public async Task ExecuteTransactionDbContextAsync(Func<DbContext, Task> func)
        {
            await this._factory.ExecuteServiceAsync(async serviceProvider =>
            {
                var dbContext = serviceProvider.GetRequiredService<DbContext>();

                await using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    await func.Invoke(dbContext);
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task ExecuteDbContextAsync(Func<DbContext, Task> func)
        {
            await this._factory.ExecuteServiceAsync(async serviceProvider =>
            {
                var dbContext = serviceProvider.GetRequiredService<DbContext>();

                await func.Invoke(dbContext);
            });
        }

        public async Task ExecuteHttpClientAsync(Func<HttpClient, Task> func)
        {
            using var httpClient = this._factory.CreateClient();
            await func.Invoke(httpClient);
        }

        public JsonSerializerOptions JsonSerializerOptions
            => this._factory.JsonSerializerSettings;

        public TModel? Parse<TModel>(string json) where TModel : class, new()
        {
            var model = default(TModel);

            if (string.IsNullOrEmpty(json)) return model;

            model = JsonSerializer.Deserialize<TModel>(json, this.JsonSerializerOptions);

            return model;
        }

        #endregion

        public async Task RepositoryExecute<TAggregate, TIdentity>(Func<IRepository<TAggregate, TIdentity>, Task> action) where TAggregate : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            await this.ExecuteServiceAsync(async serviceProvider =>
            {
                var repositoryFactory = serviceProvider.GetService<IRepositoryFactory>();
                var repository = repositoryFactory.CreateRepository<TAggregate, TIdentity>();
                await action(repository);
            });
        }

        public async Task SeedingData<TAggregate, TIdentity>(params TAggregate[] entities) where TAggregate : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            if (entities == null || !entities.Any()) return;

            await this.ExecuteDbContextAsync(async dbContext =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync();
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
            });
        }
    }
}
