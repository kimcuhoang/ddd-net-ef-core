using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCatalogQueries;

public abstract class TestCatalogQueriesBase(TestQueriesCollectionFixture testFixture, ITestOutputHelper output) : TestQueriesBase(testFixture, output)
{
    protected List<Catalog> Catalogs { get; private set; } = new();
    protected Catalog CatalogHasCatalogCategory { get; private set; } = default!;
    protected Category Category { get; private set; } = default!;
    protected Product Product { get; private set; } = default!;
    protected Catalog CatalogWithoutCatalogCategory { get; private set; } = default!;


    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());
        this.Product = Product.Create(this._fixture.Create<string>());

        this.CatalogHasCatalogCategory = Catalog.Create(this._fixture.Create<string>());
        var catalogCategory = this.CatalogHasCatalogCategory.AddCategory(this.Category.Id, this.Category.DisplayName);
        var catalogProduct = catalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);


        this.CatalogWithoutCatalogCategory = Catalog.Create(this._fixture.Create<string>());

        this.Catalogs = new List<Catalog>
        {
            this.CatalogHasCatalogCategory,
            this.CatalogWithoutCatalogCategory
        };

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.AddRange(this.Product, this.Category);
            dbContext.AddRange(this.Catalogs);
            await dbContext.SaveChangesAsync();
        });
    }
}
