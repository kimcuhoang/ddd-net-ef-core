using AutoFixture;
using DDDEfCore.Core.Common.Models;
using System;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Services.Queries.Db;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests
{
    public abstract class BaseTestFixture<TAggregate> where TAggregate : DDDEfCore.Core.Common.Models.AggregateRoot
    {
        protected SharedFixture SharedFixture { get; }
        protected IFixture Fixture { get; }

        protected BaseTestFixture(SharedFixture sharedFixture)
        {
            this.SharedFixture = sharedFixture ?? throw new ArgumentNullException(nameof(sharedFixture));
            this.Fixture = new Fixture();
        }

        protected async Task SeedingData<T>(params T[] entities) where T : AggregateRoot
        {
            await this.SharedFixture.SeedingData(entities);
        }

        public async Task ExecuteScopeAsync(Func<SqlServerDbConnectionFactory, Task> action)
        {
            await this.SharedFixture.ExecuteScopeAsync(action);
        }
    }
}
