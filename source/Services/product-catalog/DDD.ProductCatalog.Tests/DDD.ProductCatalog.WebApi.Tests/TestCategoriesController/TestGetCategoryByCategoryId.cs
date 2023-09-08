using DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryDetail;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDD.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestGetCategoryByCategoryId : TestCategoriesControllerBase
{
    public TestGetCategoryByCategoryId(WebApiTestFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    private string ApiUrl => $"{this.BaseUrl}/{(Guid)this.Category.Id}";



    [Fact(DisplayName = "Get Category By CategoryId Successfully")]
    public async Task Get_Category_By_CategoryId_Successfully()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var response = await httpClient.GetAsync(this.ApiUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var category = await this.ParseResponse<GetCategoryDetailResult>(response);

            category.ShouldNotBeNull();
            category.CategoryDetail.Id.ShouldBe(this.Category.Id);
            category.CategoryDetail.DisplayName.ShouldBe(this.Category.DisplayName);
        });
    }

    [Fact(DisplayName = "CategoryId empty should return Status400BadRequest")]
    public async Task CategoryId_Empty_Should_Return_Status400BadRequest()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var apiUrl = $"{this.BaseUrl}/{Guid.Empty}";
            var response = await httpClient.GetAsync(apiUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var errorResult = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);

            errorResult.ShouldNotBeNull();
            errorResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        });
    }
}
