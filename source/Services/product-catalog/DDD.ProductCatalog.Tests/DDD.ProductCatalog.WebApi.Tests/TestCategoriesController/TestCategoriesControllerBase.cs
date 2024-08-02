using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.WebApi.Tests.TestCategoriesController;
public abstract class TestCategoriesControllerBase(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : WebApiTestBase(testCollectionFixture, output)
{
    protected Category Category { get; private set; } = default!;

    protected string BaseUrl => $"api/categories";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(this.Category);
            await dbContext.SaveChangesAsync();
        });
    }
}
