using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogProductQueries
{
    [Collection(nameof(Tests.SharedFixture))]
    public class TestCatalogProductFixture : BaseTestFixture<Catalog>, IAsyncLifetime
    {
        public TestCatalogProductFixture(SharedFixture sharedFixture) : base(sharedFixture)
        {
        }
        public Product Product { get; private set; }
        public Category Category { get; private set; }
        public Catalog Catalog { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }
        public CatalogProduct CatalogProduct { get; private set; }

        #region Implementation of IAsyncLifetime

        public async Task InitializeAsync()
        {
            this.Category = Category.Create(this.Fixture.Create<string>());
            await this.SharedFixture.SeedingData(this.Category);

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.SharedFixture.SeedingData(this.Product);

            this.Catalog = Catalog.Create(this.Fixture.Create<string>());
            this.CatalogCategory = this.Catalog.AddCategory(this.Category.CategoryId, this.Category.DisplayName);
            this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Product.Name);

            await this.SharedFixture.SeedingData(this.Catalog);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion
    }
}
