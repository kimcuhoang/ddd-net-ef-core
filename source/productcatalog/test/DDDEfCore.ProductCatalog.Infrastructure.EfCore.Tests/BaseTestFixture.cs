using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
{
    public abstract class BaseTestFixture<TAggregate> where TAggregate : AggregateRoot
    {
        protected SharedFixture SharedFixture { get; }
        protected IFixture Fixture { get; }

        protected BaseTestFixture(SharedFixture sharedFixture)
        {
            this.SharedFixture = sharedFixture ?? throw new ArgumentNullException(nameof(sharedFixture));
            this.Fixture = new Fixture();
        }

        public async Task RepositoryExecute(Func<IRepository<TAggregate>, Task> action)
        {
            await this.SharedFixture.ExecuteScopeAsync(async (services) =>
            {
                var repositoryFactory = services.GetService<IRepositoryFactory>();
                var repository = repositoryFactory.CreateRepository<TAggregate>();
                await action(repository);
            });
        }

        public abstract Task InitData();

        public abstract Task DoAssert(Action<TAggregate> assertFor);
    }
}
