using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController
{
    [Collection(nameof(SharedFixture))]
    public class TestUpdateCategory : IClassFixture<TestCategoryControllerFixture>
    {
        private readonly TestCategoryControllerFixture _testCategoryControllerFixture;

        public TestUpdateCategory(TestCategoryControllerFixture testCategoryControllerFixture)
            => this._testCategoryControllerFixture = testCategoryControllerFixture;

        private Category Category => this._testCategoryControllerFixture.Category;
        public string ApiUrl => $"{this._testCategoryControllerFixture.BaseUrl}/{(Guid)this.Category.CategoryId}";

        [Theory(DisplayName = "Update Category Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Update_Category_Successfully_Should_Return_HttpStatusCode204(string categoryName)
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions, services) =>
            {
                var jsonContent = JsonSerializer.Serialize(categoryName);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Update Category with Empty Name Should Return HttpStatusCode400")]
        public async Task Update_Category_With_Empty_Name_Should_Return_HttpStatusCode400()
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions, services) =>
            {
                var jsonContent = JsonSerializer.Serialize(string.Empty);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            });
        }
    }
}
