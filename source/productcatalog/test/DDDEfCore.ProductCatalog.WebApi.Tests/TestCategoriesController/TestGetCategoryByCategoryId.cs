using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCategoryByCategoryId : IClassFixture<TestCategoryControllerFixture>
    {
        private readonly TestCategoryControllerFixture _testCategoryControllerFixture;

        private Category Category => this._testCategoryControllerFixture.Category;

        private string ApiUrl => $"{this._testCategoryControllerFixture.BaseUrl}/{(Guid) this.Category.CategoryId}";

        public TestGetCategoryByCategoryId(TestCategoryControllerFixture testCategoryControllerFixture)
            => this._testCategoryControllerFixture = testCategoryControllerFixture;

        [Fact(DisplayName = "Get Category By CategoryId Successfully")]
        public async Task Get_Category_By_CategoryId_Successfully()
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions) =>
            {
                var response = await client.GetAsync(this.ApiUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                
                var result = await response.Content.ReadAsStringAsync();
                var category = JsonSerializer.Deserialize<GetCategoryDetailResult>(result, jsonSerializeOptions);
                
                category.ShouldNotBeNull();
                category.CategoryDetail.Id.ShouldBe(this.Category.CategoryId);
                category.CategoryDetail.DisplayName.ShouldBe(this.Category.DisplayName);
            });
        }

        [Fact(DisplayName = "CategoryId empty should return Status400BadRequest")]
        public async Task CategoryId_Empty_Should_Return_Status400BadRequest()
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions) =>
            {
                var apiUrl = $"{this._testCategoryControllerFixture.BaseUrl}/{Guid.Empty}";
                var response = await client.GetAsync(apiUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

                var result = await response.Content.ReadAsStringAsync();
                var errorResult = JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result, jsonSerializeOptions);

                errorResult.ShouldNotBeNull();
                errorResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            });
        }
    }
}
