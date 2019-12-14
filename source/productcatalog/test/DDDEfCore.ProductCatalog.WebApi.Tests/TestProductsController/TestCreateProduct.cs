using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController
{
    [Collection(nameof(SharedFixture))]
    public class TestCreateProduct : IClassFixture<TestProductsControllerFixture>
    {
        private readonly TestProductsControllerFixture _testProductsControllerFixture;

        private string ApiUrl => this._testProductsControllerFixture.BaseUrl;

        public TestCreateProduct(TestProductsControllerFixture testProductsControllerFixture)
            => this._testProductsControllerFixture = testProductsControllerFixture;

        [Theory(DisplayName = "Create Product Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Create_Product_Successfully_Should_Return_HttpStatusCode204(string productName)
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var command = new CreateProductCommand
                {
                    ProductName = productName
                };

                var jsonData = JsonSerializer.Serialize(command);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);

                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Create Product With Invalid Request Should Return HttpStatusCode400")]
        public async Task Create_Product_With_Invalid_Request_Should_Return_HttpStatusCode400()
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var command = new CreateProductCommand();

                var jsonData = JsonSerializer.Serialize(command);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

                var result = await response.Content.ReadAsStringAsync();
                var errorResponse =
                    JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                        jsonSerializationOptions);

                errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
                errorResponse.ErrorMessages.ShouldNotBeEmpty();
            });
        }
    }
}
