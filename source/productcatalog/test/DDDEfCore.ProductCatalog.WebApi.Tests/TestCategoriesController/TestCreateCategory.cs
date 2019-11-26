using AutoFixture.Xunit2;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController
{
    [Collection(nameof(SharedFixture))]
    public class TestCreateCategory : IClassFixture<TestCategoryControllerFixture>
    {
        private readonly TestCategoryControllerFixture _testCategoryControllerFixture;
        private string ApiUrl => $"{this._testCategoryControllerFixture.BaseUrl}/create";

        public TestCreateCategory(TestCategoryControllerFixture testCategoryControllerFixture)
        {
            this._testCategoryControllerFixture = testCategoryControllerFixture;
        }

        [Theory(DisplayName = "Create Category Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Create_Category_Successfully_Should_Return_HttpStatusCode204(string categoryName)
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions, services) =>
            {
                var jsonContent = JsonSerializer.Serialize(new
                {
                    CategoryName = categoryName
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Create Category With Empty Name Should Return HttpStatusCode400")]
        public async Task Create_Category_With_EmptyName_Should_Return_HttpStatusCode400()
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions, services) =>
            {
                var jsonContent = JsonSerializer.Serialize(new
                {
                    CategoryName = string.Empty
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            });
        }
    }
}
