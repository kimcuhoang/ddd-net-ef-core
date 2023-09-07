using AutoFixture;
using DDD.ProductCatalog.WebApi.Tests;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestCategoryControllerFixture : DefaultTestFixture
{
    public TestCategoryControllerFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Category Category { get; private set; }

    public string BaseUrl => $"api/categories";

    #region Overrides of SharedFixture

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this.AutoFixture.Create<string>());
        await this.SeedingData<Category, CategoryId>(this.Category);
    }

    #endregion
}
