using DDD.ProductCatalog.Core.Products;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestProductQueries;

public abstract class TestProductQueriesBase(TestQueriesCollectionFixture testFixture, ITestOutputHelper output) : TestQueriesBase(testFixture, output)
{
    protected Product Product { get; private set; } = default!;
    protected Category Category { get; private set; } = default!;
    protected Catalog Catalog { get; private set; } = default!;
    protected CatalogCategory CatalogCategory { get; private set; } = default!;
    protected CatalogProduct CatalogProduct { get; private set; } = default!;

    #region Overrides of SharedFixture

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
            dbContext.AddRange(this.Product, this.Category, this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    #endregion
}
