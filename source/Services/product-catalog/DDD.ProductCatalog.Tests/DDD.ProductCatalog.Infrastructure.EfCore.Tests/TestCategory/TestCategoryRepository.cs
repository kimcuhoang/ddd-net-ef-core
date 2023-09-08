using DDD.ProductCatalog.Core.Categories;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory;

public class TestCategoryRepository : TestEfCoreBase
{
    public TestCategoryRepository(TestEfCoreFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    [Fact(DisplayName = "Should Create Category Successfully")]
    public async Task ShouldCreateCategorySuccessfully()
    {
        var category = Category.Create(this._fixture.Create<string>());

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(category);
            await dbContext.SaveChangesAsync();
        });

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var categorySaved = await dbContext.Set<Category>().FirstOrDefaultAsync(_ => _ == category);
            categorySaved.ShouldNotBeNull();
            categorySaved.Equals(category).ShouldBeTrue();
        });
    }
}
