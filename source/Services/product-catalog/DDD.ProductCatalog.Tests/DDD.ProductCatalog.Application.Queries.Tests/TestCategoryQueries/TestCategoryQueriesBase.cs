using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCategoryQueries;

public abstract class TestCategoryQueriesBase : TestQueriesBase
{
    protected TestCategoryQueriesBase(TestQueriesFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    protected Category Category { get; private set; } = default!;
    protected Catalog Catalog { get; private set; } = default!;
    protected CatalogCategory CatalogCategory { get; private set; } = default!;

    #region Overrides of SharedFixture

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());
        this.Catalog = Catalog.Create(this._fixture.Create<string>());
        
        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.AddRange(this.Category, this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    #endregion
}
