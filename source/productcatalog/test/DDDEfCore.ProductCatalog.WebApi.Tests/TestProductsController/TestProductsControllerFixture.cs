using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestProductsControllerFixture : DefaultTestFixture
{
    public TestProductsControllerFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

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
        await this.SeedingData<Product,ProductId>(this.Product);

        this.Category = Category.Create(this.AutoFixture.Create<string>());
        await this.SeedingData<Category,CategoryId>(this.Category);

        this.Catalog = Catalog.Create(this.AutoFixture.Create<string>());
        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);
        await this.SeedingData<Catalog,CatalogId>(this.Catalog);
    }

    #endregion
}
