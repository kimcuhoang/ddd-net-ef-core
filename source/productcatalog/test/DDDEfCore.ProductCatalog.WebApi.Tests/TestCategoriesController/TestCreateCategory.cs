using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System.Net;
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
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions) =>
            {
                var content = ContentHelper.GetStringContent(new
                {
                    CategoryName = categoryName
                });
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Create Category With Empty Name Should Return HttpStatusCode400")]
        public async Task Create_Category_With_EmptyName_Should_Return_HttpStatusCode400()
        {
            await this._testCategoryControllerFixture.DoTest(async (client, jsonSerializeOptions) =>
            {
                var content = ContentHelper.GetStringContent(new
                {
                    CategoryName = string.Empty
                });
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            });
        }
    }
}
