using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Application.Commands.CategoryCommands.CreateCategory;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestCreateCategory : TestCategoriesControllerBase
{
    public TestCreateCategory(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    private string ApiUrl => $"{this.BaseUrl}/create";

    [Theory(DisplayName = "Create Category Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Create_Category_Successfully_Should_Return_HttpStatusCode204(string categoryName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(new { CategoryName = categoryName });
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var model = await this.ParseResponse<CreateCategoryResult>(response);

            model.ShouldNotBeNull();
            model.CategoryId.ShouldNotBeNull().ShouldNotBe(CategoryId.Empty);

            await this.ExecuteDbContextAsync(async dbContext =>
            {
                var category = await dbContext.Set<Category>().FirstOrDefaultAsync(_ => _.Id == model.CategoryId);

                category.ShouldNotBeNull();
            });
        });


    }

    [Fact(DisplayName = "Create Category With Empty Name Should Return HttpStatusCode400")]
    public async Task Create_Category_With_EmptyName_Should_Return_HttpStatusCode400()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(string.Empty);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        });
    }
}
