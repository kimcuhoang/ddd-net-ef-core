using AutoFixture.Xunit2;
using DDD.ProductCatalog.Infrastructure.EfCore.Tests;
using DDD.ProductCatalog.Core.Categories;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory;

public class TestCategoryRepository : TestBase<TestCategoryFixture>
{
    public TestCategoryRepository(ITestOutputHelper testOutput, TestCategoryFixture fixture) : base(testOutput, fixture)
    {
    }

    [Fact(DisplayName = "Should Create Category Successfully")]
    public async Task ShouldCreateCategorySuccessfully()
    {
        await this._fixture.DoAssert(category =>
        {
            category.ShouldNotBeNull();
            category.Equals(this._fixture.Category).ShouldBeTrue();
        });
    }

    [Theory(DisplayName = "Should Update Category Successfully")]
    [AutoData]
    public async Task ShouldUpdateCategorySuccessfully(string newCategoryName)
    {
        await this._fixture.RepositoryExecute<Category, CategoryId>(async repository =>
        {
            var category = await repository.FindOneAsync(x => x.Id == this._fixture.Category.Id);

            category.ShouldNotBeNull();

            category.ChangeDisplayName(newCategoryName);

            //repository.Update(category);
        });

        await this._fixture.DoAssert(category =>
        {
            category.ShouldNotBeNull();
            category.Equals(this._fixture.Category).ShouldBeTrue();
            category.DisplayName.ShouldBe(newCategoryName);
        });
    }

    [Fact(DisplayName = "Should Remove Category Successfully")]
    public async Task ShouldRemoveCategorySuccessfully()
    {
        await this._fixture.RepositoryExecute<Category, CategoryId>(async repository =>
        {
            var category = await repository.FindOneAsync(x => x == this._fixture.Category);
            category.ShouldNotBeNull();
            repository.Remove(category);
        });

        await this._fixture.DoAssert(category =>
        {
            category.ShouldBeNull();
        });
    }
}
