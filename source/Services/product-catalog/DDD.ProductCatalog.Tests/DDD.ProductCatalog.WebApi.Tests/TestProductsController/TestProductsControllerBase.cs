using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.WebApi.Tests.TestProductsController;
public abstract class TestProductsControllerBase : WebApiTestBase
{
    protected TestProductsControllerBase(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    protected string BaseUrl => $"api/products";
    protected Product Product { get; private set; } = default!;
    protected Category Category { get; private set; } = default!;
    protected Catalog Catalog { get; private set; } = default!;
    protected CatalogCategory CatalogCategory { get; private set; } = default!;
    protected CatalogProduct CatalogProduct { get; private set; } = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Product = Product.Create(this._fixture.Create<string>());

        this.Category = Category.Create(this._fixture.Create<string>());

        this.Catalog = Catalog.Create(this._fixture.Create<string>());
        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(this.Product);
            dbContext.Add(this.Category);
            dbContext.Add(this.Catalog);

            await dbContext.SaveChangesAsync();
        });
    }
}
