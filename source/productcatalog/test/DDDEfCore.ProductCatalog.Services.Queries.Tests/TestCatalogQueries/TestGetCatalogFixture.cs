using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries
{
    public class TestGetCatalogFixture : SharedFixture
    {
        public List<Catalog> Catalogs { get; private set; }
        public Catalog CatalogHasCatalogCategory { get; private set; }
        public Category Category { get; private set; }
        public Product Product { get; private set; }
        public Catalog CatalogWithoutCatalogCategory { get; private set; }


        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Category = Category.Create(this.Fixture.Create<string>());
            await this.SeedingData(this.Category);

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.SeedingData(this.Product);

            this.CatalogHasCatalogCategory = Catalog.Create(this.Fixture.Create<string>()); 
            var catalogCategory = this.CatalogHasCatalogCategory.AddCategory(this.Category.CategoryId, this.Category.DisplayName);
            var catalogProduct = catalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Product.Name);


            this.CatalogWithoutCatalogCategory = Catalog.Create(this.Fixture.Create<string>());

            this.Catalogs = new List<Catalog>
            {
                this.CatalogHasCatalogCategory,
                this.CatalogWithoutCatalogCategory
            };

            await this.SeedingData(this.Catalogs.ToArray());
        }
    }
}
