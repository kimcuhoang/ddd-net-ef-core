using DDD.ProductCatalog.Application.Commands.CategoryCommands.UpdateCategory;
using DDD.ProductCatalog.Core.Categories;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestUpdateCategory : TestCategoriesControllerBase
{
    public TestUpdateCategory(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    public string ApiUrl => $"{this.BaseUrl}/{(Guid)this.Category.Id}";

    [Theory(DisplayName = "Update Category Successfully")]
    [AutoData]
    public async Task Update_Category_Successfully_Should_Return(string categoryName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(categoryName);
            var response = await httpClient.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var model = await this.ParseResponse<UpdateCategoryResult>(response);

            model.ShouldNotBeNull();
            model.CategoryId.ShouldBe(this.Category.Id);

            await this.ExecuteDbContextAsync(async dbContext =>
            {
                var category = await dbContext.Set<Category>().FirstOrDefaultAsync(_ => _ == this.Category);

                category.ShouldNotBeNull();
                category.DisplayName.ShouldBe(categoryName);
            });
        });
    }

    [Fact(DisplayName = "Update Category with Empty Name Should Return HttpStatusCode400")]
    public async Task Update_Category_With_Empty_Name_Should_Return_HttpStatusCode400()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(string.Empty);
            var response = await httpClient.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        });
    }
}
