using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Net;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestGetCategoryByCategoryId : TestBase<TestCategoryControllerFixture>
{
    public TestGetCategoryByCategoryId(ITestOutputHelper testOutput, TestCategoryControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private Category Category => this._fixture.Category;

    private string ApiUrl => $"{this._fixture.BaseUrl}/{(Guid) this.Category.Id}";

    

    [Fact(DisplayName = "Get Category By CategoryId Successfully")]
    public async Task Get_Category_By_CategoryId_Successfully()
    {
        await this._fixture.DoTest(async (client, jsonSerializeOptions) =>
        {
            var response = await client.GetAsync(this.ApiUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            
            var result = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<GetCategoryDetailResult>(result, jsonSerializeOptions);
            
            category.ShouldNotBeNull();
            category.CategoryDetail.Id.ShouldBe(this.Category.Id);
            category.CategoryDetail.DisplayName.ShouldBe(this.Category.DisplayName);
        });
    }

    [Fact(DisplayName = "CategoryId empty should return Status400BadRequest")]
    public async Task CategoryId_Empty_Should_Return_Status400BadRequest()
    {
        await this._fixture.DoTest(async (client, jsonSerializeOptions) =>
        {
            var apiUrl = $"{this._fixture.BaseUrl}/{Guid.Empty}";
            var response = await client.GetAsync(apiUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();
            var errorResult = JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result, jsonSerializeOptions);

            errorResult.ShouldNotBeNull();
            errorResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
        });
    }
}
