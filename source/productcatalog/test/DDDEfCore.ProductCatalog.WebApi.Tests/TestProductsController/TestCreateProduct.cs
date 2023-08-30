using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestCreateProduct : TestBase<TestProductsControllerFixture>
{
    public TestCreateProduct(ITestOutputHelper testOutput, TestProductsControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private string ApiUrl => this._fixture.BaseUrl;

    

    [Theory(DisplayName = "Create Product Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Create_Product_Successfully_Should_Return_HttpStatusCode204(string productName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var command = new CreateProductCommand
            {
                ProductName = productName
            };

            var content = command.ToStringContent();
            var response = await client.PostAsync(this.ApiUrl, content);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        });
    }

    [Fact(DisplayName = "Create Product With Invalid Request Should Return HttpStatusCode400")]
    public async Task Create_Product_With_Invalid_Request_Should_Return_HttpStatusCode400()
    {
        await this._fixture.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateProductCommand();

            var content = this.ConvertToStringContent(command);

            var response = await httpClient.PostAsync(this.ApiUrl, content);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();

            var errorResult = this._fixture.Parse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result);

            errorResult.ShouldNotBeNull();
            errorResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResult.ErrorMessages.ShouldNotBeEmpty();
        });
    }
}
