using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;
using DDD.ProductCatalog.Application.Commands.ProductCommands.UpdateProduct;

namespace DDD.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestUpdateProduct : TestProductsControllerBase
{
    public TestUpdateProduct(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    private string ApiUrl => $"{this.BaseUrl}/{(Guid)this.Product.Id}";

    [Theory(DisplayName = "Update Product Successfully")]
    [AutoData]
    public async Task Update_Product_Successfully_Should_Return(string productName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(productName);
            var response = await httpClient.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var model = await this.ParseResponse<UpdateProductResult>(response);
            model.ShouldNotBeNull();
            model.ProductId.ShouldBe(this.Product.Id);
        });

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var product = await dbContext.Set<Product>().FirstOrDefaultAsync(_ => _ == this.Product);

            product.ShouldNotBeNull();
            product.Name.ShouldBe(productName);
        });
    }

    [Fact(DisplayName = "Update Product With Invalid Request Should Return HttpStatusCode400")]
    public async Task Update_Product_With_Invalid_Request_Should_Return_HttpStatusCode400()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(string.Empty);
            var response = await httpClient.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var errorResponse = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }
}
