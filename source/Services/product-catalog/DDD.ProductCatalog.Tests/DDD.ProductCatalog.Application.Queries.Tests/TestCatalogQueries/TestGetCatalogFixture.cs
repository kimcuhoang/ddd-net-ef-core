using AutoFixture;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCatalogQueries;

public class TestGetCatalogFixture : DefaultTestFixture
{
    public TestGetCatalogFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public List<Catalog> Catalogs { get; private set; } = new();
    public Catalog CatalogHasCatalogCategory { get; private set; } = default!;
    public Category Category { get; private set; } = default!;
    public Product Product { get; private set; } = default!;
    public Catalog CatalogWithoutCatalogCategory { get; private set; } = default!;


    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this.Fixture.Create<string>());
        await this.SeedingData<Category, CategoryId>(this.Category);

        this.Product = Product.Create(this.Fixture.Create<string>());
        await this.SeedingData<Product, ProductId>(this.Product);

        this.CatalogHasCatalogCategory = Catalog.Create(this.Fixture.Create<string>());
        var catalogCategory = this.CatalogHasCatalogCategory.AddCategory(this.Category.Id, this.Category.DisplayName);
        var catalogProduct = catalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);


        this.CatalogWithoutCatalogCategory = Catalog.Create(this.Fixture.Create<string>());

        this.Catalogs = new List<Catalog>
        {
            this.CatalogHasCatalogCategory,
            this.CatalogWithoutCatalogCategory
        };

        await this.SeedingData<Catalog, CatalogId>(this.Catalogs.ToArray());
    }
}
