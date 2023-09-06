using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestUpdateProduct : TestBase<TestProductsControllerFixture>
{
    public TestUpdateProduct(ITestOutputHelper testOutput, TestProductsControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private Product Product => this._fixture.Product;
    private string ApiUrl => $"{this._fixture.BaseUrl}/{(Guid)this.Product.Id}";

   

    [Theory(DisplayName = "Update Product Successfully")]
    [AutoData]
    public async Task Update_Product_Successfully_Should_Return(string productName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var content = productName.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        });

        await this._fixture.ExecuteDbContextAsync(async dbContext =>
        {
            var product = await dbContext.Set<Product>().FirstOrDefaultAsync(_ => _.Id == this._fixture.Product.Id);

            product.ShouldNotBeNull();
            product.Name.ShouldBe(productName);
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
