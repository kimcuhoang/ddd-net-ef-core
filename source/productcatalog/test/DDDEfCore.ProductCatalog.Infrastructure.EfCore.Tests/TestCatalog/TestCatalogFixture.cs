using System;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogFixture : IAsyncLifetime
    {
        private readonly SharedFixture _sharedFixture;
        private readonly IFixture _fixture;

        public TestCatalogFixture(SharedFixture sharedFixture)
        {
            this._sharedFixture = sharedFixture ?? throw new ArgumentNullException(nameof(sharedFixture));
            this._fixture = new Fixture();
        }

        public Catalog Catalog { get; private set; }
        public Category CategoryLv1 { get; private set; }
        public Category CategoryLv2 { get; private set; }
        public Category CategoryLv3 { get; private set; }

        #region Implementation of IAsyncLifetime

        public async Task InitializeAsync()
        {
            this.CategoryLv1 = Category.Create(this._fixture.Create<string>());
            this.CategoryLv2 = Category.Create(this._fixture.Create<string>());
            this.CategoryLv3 = Category.Create(this._fixture.Create<string>());

            await this._sharedFixture.SeedingData(this.CategoryLv1, this.CategoryLv2, this.CategoryLv3);

            this.Catalog = Catalog.Create(this._fixture.Create<string>());

            var subCategoryLv1 = this.Catalog.AddCategoryRoot(this.CategoryLv1.CategoryId);
            subCategoryLv1.WithDisplayName("Lv1");

            var subCategoryLv2 = subCategoryLv1.AddSubCategory(this.CategoryLv2.CategoryId);
            subCategoryLv2.WithDisplayName("Lv2");
            var subCategoryLv3 = subCategoryLv2.AddSubCategory(this.CategoryLv3.CategoryId);
            subCategoryLv3.WithDisplayName("Lv3");

            await this.RepositoryExecuteAsync(async repository => { await repository.AddAsync(this.Catalog); });
        }

        public Task DisposeAsync() => Task.CompletedTask;


        #endregion

        public async Task RepositoryExecuteAsync(Func<IRepository<Catalog>, Task> action)
        {
            await this._sharedFixture.ExecuteScopeAsync(async serviceProvider =>
            {
                var repositoryFactory = serviceProvider.GetService<IRepositoryFactory>();
                var repository = repositoryFactory.CreateRepository<Catalog>();
                await action(repository);
            });
        }
    }
}
