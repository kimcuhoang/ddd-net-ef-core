using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Xunit;
using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory
{
    [Collection(nameof(Tests.SharedFixture))]
    public class TestCatalogCategoryFixture : BaseTestFixture<Catalog>, IAsyncLifetime
    {
        public TestCatalogCategoryFixture(SharedFixture sharedFixture) : base(sharedFixture) { }

        public Catalog Catalog { get; private set; }
        public Category Category { get; private set; }
        public Product Product { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }
        public CatalogProduct CatalogProduct { get; private set; }

        #region Overrides of BaseTestFixture<Catalog>

        public override async Task InitData()
        {
            this.Catalog = Catalog.Create(this.Fixture.Create<string>());

            this.CatalogCategory = 
                this.Catalog.AddCategory(this.Category.CategoryId, this.Fixture.Create<string>());
            
            await this.RepositoryExecute(async repository =>
            {
                await repository.AddAsync(this.Catalog);
            });
        }

        public override async Task DoAssert(Action<Catalog> assertFor)
        {
            await this.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                assertFor(catalog);
            });
        }

        #endregion

        #region Implementation of IAsyncLifetime

        public async Task InitializeAsync()
        {
            this.Category = Category.Create(this.Fixture.Create<string>());
            await this.SharedFixture.SeedingData(this.Category);

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.SharedFixture.SeedingData(this.Product);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        public async Task InitDataFull()
        {
            this.Catalog = Catalog.Create(this.Fixture.Create<string>());

            this.CatalogCategory =
                this.Catalog.AddCategory(this.Category.CategoryId, this.Fixture.Create<string>());

            this.CatalogProduct =
                this.CatalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Fixture.Create<string>());

            await this.RepositoryExecute(async repository =>
            {
                await repository.AddAsync(this.Catalog);
            });
        }

        public async Task DoActionWithCatalogCategory(Action<CatalogCategory> action)
        {
            await this.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

                action(catalogCategory);

                await repository.UpdateAsync(catalog);
            });
        }

        public async Task DoAssertForCatalogCategory(Action<CatalogCategory> action)
        {
            await this.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

                action(catalogCategory);
            });
        }
    }
}
