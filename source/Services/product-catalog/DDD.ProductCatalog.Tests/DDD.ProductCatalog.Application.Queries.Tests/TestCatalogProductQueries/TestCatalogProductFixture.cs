using AutoFixture;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCatalogProductQueries;

public class TestCatalogProductFixture : DefaultTestFixture
{
    public TestCatalogProductFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Product Product { get; private set; } = default!;
    public Category Category { get; private set; } = default!;
    public Catalog Catalog { get; private set; } = default!;
    public CatalogCategory CatalogCategory { get; private set; } = default!;
    public CatalogProduct CatalogProduct { get; private set; } = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this.Fixture.Create<string>());
        await this.SeedingData<Category, CategoryId>(this.Category);

        this.Product = Product.Create(this.Fixture.Create<string>());
        await this.SeedingData<Product, ProductId>(this.Product);

        this.Catalog = Catalog.Create(this.Fixture.Create<string>());
        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);

        await this.SeedingData<Catalog, CatalogId>(this.Catalog);
    }


}
