using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.WebApi.Tests.TestCatalogsController;
public abstract class TestCatalogsControllerBase(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : WebApiTestBase(testCollectionFixture, output)
{
    public string BaseUrl => @"api/catalogs";
    public Catalog Catalog { get; private set; } = default!;
    public Category Category { get; private set; } = default!;
    public Product Product { get; private set; } = default!;
    public CatalogCategory CatalogCategory { get; private set; } = default!;


    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());
        this.Product = Product.Create(this._fixture.Create<string>());
        this.Catalog = Catalog.Create(this._fixture.Create<string>());

        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(this.Category);
            dbContext.Add(this.Product);
            dbContext.Add(this.Catalog);

            await dbContext.SaveChangesAsync();
        });
    }
}
