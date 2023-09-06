using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory;

public class TestCategoryFixture : DefaultTestFixture
{
    public TestCategoryFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Category Category { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this.Fixture.Create<string>());

        await this.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(this.Category);
            await dbContext.SaveChangesAsync();
        });
    }

    public async Task DoAssert(Action<Category> assertFor)
    {
        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var category = await dbContext.Set<Category>().FirstOrDefaultAsync(_ => _ == this.Category);

            assertFor(category);
        });
    }

}
