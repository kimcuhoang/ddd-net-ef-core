using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogProduct
{
    [Collection(nameof(Tests.SharedFixture))]
    public class TestCatalogProductFixture : BaseTestFixture<Catalog>, IAsyncLifetime
    {
        public TestCatalogProductFixture(SharedFixture sharedFixture) : base(sharedFixture) { }

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

            this.CatalogProduct =
                this.CatalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Fixture.Create<string>());

            await this.RepositoryExecute(async repository => { await repository.AddAsync(this.Catalog); });
        }

        public override Task DoAssert(Action<Catalog> assertFor) => Task.CompletedTask;

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

        public async Task DoActionWithCatalogProduct(Action<CatalogProduct> action)
        {
            await this.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x == this.CatalogProduct);

                action(catalogProduct);

                await repository.UpdateAsync(catalog);
            });
        }

        public async Task DoAssertForCatalogProduct(Action<CatalogProduct> action)
        {
            await this.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x == this.CatalogProduct);

                action(catalogProduct);
            });
        }
    }
}
