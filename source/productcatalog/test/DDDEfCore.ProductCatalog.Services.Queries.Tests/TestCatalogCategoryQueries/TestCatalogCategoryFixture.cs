using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using GenFu;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogCategoryQueries
{
    [Collection(nameof(Tests.SharedFixture))]
    public class TestCatalogCategoryFixture : SharedFixture
    {
        public Catalog Catalog { get; private set; }

        public CatalogCategory CatalogCategory => this.Catalog.Categories.FirstOrDefault();

        public List<CatalogProduct> CatalogProducts => this.CatalogCategory.Products.ToList();

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Catalog = this.CreateCatalog();
            await this.SeedingData(this.Catalog);
        }

        private Catalog CreateCatalog()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this.Fixture.Create<string>());
            
            var totalCatalogProducts = A.Random.Next(1, 5);
            Enumerable.Range(0, totalCatalogProducts).ToList().ForEach(idx =>
            {
                var productId = IdentityFactory.Create<ProductId>();
                catalogCategory.CreateCatalogProduct(productId, this.Fixture.Create<string>());
            });

            return catalog;
        }
    }
}
