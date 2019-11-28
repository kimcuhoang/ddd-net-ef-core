using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController
{
    [Collection(nameof(SharedFixture))]
    public class TestProductsControllerFixture : SharedFixture
    {
        public string BaseUrl => $"api/products";
        public Product Product { get; private set; }
        public Category Category { get; private set; }
        public Catalog Catalog { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }
        public CatalogProduct CatalogProduct { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Product = Product.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Product);

            this.Category = Category.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Category);

            this.Catalog = Catalog.Create(this.AutoFixture.Create<string>());
            this.CatalogCategory = this.Catalog.AddCategory(this.Category.CategoryId, this.Category.DisplayName);
            this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Product.Name);
            await this.SeedingData(this.Catalog);
        }

        #endregion
    }
}
