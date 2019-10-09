using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    [Collection(nameof(SharedFixture))]
    public class SharedRepositoryFixture<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly SharedFixture _sharedFixture;

        public SharedRepositoryFixture(SharedFixture sharedFixture)
            => this._sharedFixture = sharedFixture ?? throw new ArgumentNullException(nameof(sharedFixture));

        public async Task RepositoryExecute(Func<IRepository<TAggregate>, Task> action)
        {
            await this._sharedFixture.ExecuteScopeAsync(async (services) =>
            {
                var repositoryFactory = services.GetService<IRepositoryFactory>();
                var repository = repositoryFactory.CreateRepository<TAggregate>();
                await action(repository);
            });
        }

        public async Task<TAggregate> RepositoryQuery(Func<IRepository<TAggregate>, Task<TAggregate>> action)
        {
            return await this._sharedFixture.QueryScopeAsync(async (services) =>
            {
                var repositoryFactory = services.GetService<IRepositoryFactory>();
                var repository = repositoryFactory.CreateRepository<TAggregate>();
                return await action(repository);
            });
        }
    }
}
