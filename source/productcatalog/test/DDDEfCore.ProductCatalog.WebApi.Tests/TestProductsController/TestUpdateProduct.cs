using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System.Net;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestUpdateProduct : TestBase<TestProductsControllerFixture>
{
    public TestUpdateProduct(ITestOutputHelper testOutput, TestProductsControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private Product Product => this._fixture.Product;
    private string ApiUrl => $"{this._fixture.BaseUrl}/{(Guid)this.Product.Id}";

   

    [Theory(DisplayName = "Update Product Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Update_Product_Successfully_Should_Return_HttpStatusCode204(string productName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var content = productName.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        });
    }

    [Fact(DisplayName = "Update Product With Invalid Request Should Return HttpStatusCode400")]
    public async Task Update_Product_With_Invalid_Request_Should_Return_HttpStatusCode400()
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var content = string.Empty.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadAsStringAsync();
            var errorResponse =
                JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                    jsonSerializerOptions);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }
}
