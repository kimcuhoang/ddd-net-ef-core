using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries
{
    public class TestGetCatalogFixture : SharedFixture
    {
        public List<Catalog> Catalogs { get; private set; }

        public Catalog CatalogWithoutCatalogCategory { get; private set; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Catalogs = new List<Catalog>();

            var numberOfCatalogs = GenFu.GenFu.Random.Next(10);
            Enumerable.Range(0, numberOfCatalogs).ToList().ForEach(i =>
            {
                var catalog = this.CreateCatalog(GenFu.GenFu.Random.Next(5));
                this.Catalogs.Add(catalog);
            });

            this.CatalogWithoutCatalogCategory = Catalog.Create(this.Fixture.Create<string>());
            this.Catalogs.Add(this.CatalogWithoutCatalogCategory);

            await this.SeedingData(this.Catalogs.ToArray());
        }

        private Catalog CreateCatalog(int numberOfCategories)
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            Enumerable.Range(0, numberOfCategories).ToList().ForEach(i =>
            {
                var categoryId = IdentityFactory.Create<CategoryId>();
                var catalogCategory = catalog.AddCategory(categoryId, this.Fixture.Create<string>());
                var numberOfProducts = GenFu.GenFu.Random.Next(0, 5);
                catalogCategory = this.AddCatalogProducts(catalogCategory, numberOfProducts);
            });

            return catalog;
        }

        private CatalogCategory AddCatalogProducts(CatalogCategory catalogCategory, int numberOfProducts)
        {
            Enumerable.Range(0, numberOfProducts).ToList().ForEach(idx =>
            {
                var productId = IdentityFactory.Create<ProductId>();
                catalogCategory.CreateCatalogProduct(productId, this.Fixture.Create<string>());
            });
            return catalogCategory;
        }
    }
}
