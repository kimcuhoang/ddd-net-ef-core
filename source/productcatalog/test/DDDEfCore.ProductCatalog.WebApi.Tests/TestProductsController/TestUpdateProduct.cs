using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController
{
    [Collection(nameof(SharedFixture))]
    public class TestUpdateProduct : IClassFixture<TestProductsControllerFixture>
    {
        private readonly TestProductsControllerFixture _testProductsControllerFixture;

        private Product Product => this._testProductsControllerFixture.Product;
        private string ApiUrl => $"{this._testProductsControllerFixture.BaseUrl}/{(Guid)this.Product.ProductId}";

        public TestUpdateProduct(TestProductsControllerFixture testProductsControllerFixture)
            => this._testProductsControllerFixture = testProductsControllerFixture;

        [Theory(DisplayName = "Update Product Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Update_Product_Successfully_Should_Return_HttpStatusCode204(string productName)
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var content = ContentHelper.GetStringContent(productName);
                var response = await client.PutAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Update Product With Invalid Request Should Return HttpStatusCode400")]
        public async Task Update_Product_With_Invalid_Request_Should_Return_HttpStatusCode400()
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var content = ContentHelper.GetStringContent(string.Empty);
                var response = await client.PutAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                var result = await response.Content.ReadAsStringAsync();
                var errorResponse =
                    JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                        jsonSerializationOptions);

                errorResponse.ShouldNotBeNull();
                errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
                errorResponse.ErrorMessages.ShouldNotBeEmpty();
            });
        }
    }
}
