using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestProductQueries
{
    public class TestProductsFixture : SharedFixture
    {
        public Product Product { get; private set; }
        public Category Category { get; private set; }
        public Catalog Catalog { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }
        public CatalogProduct CatalogProduct { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.SeedingData<Product,ProductId>(this.Product);

            this.Category = Category.Create(this.Fixture.Create<string>());
            await this.SeedingData<Category,CategoryId>(this.Category);

            this.Catalog = Catalog.Create(this.Fixture.Create<string>());
            this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
            this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);
            await this.SeedingData<Catalog,CatalogId>(this.Catalog);
        }

        #endregion
    }
}
