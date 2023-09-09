using DDD.ProductCatalog.Core.Products;
using DDD.ProductCatalog.Application.Commands.ProductCommands.CreateProduct;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestCreateProduct : TestProductsControllerBase
{
    public TestCreateProduct(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    private string ApiUrl => this.BaseUrl;



    [Theory(DisplayName = "Create Product Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Create_Product_Successfully_Should_Return_HttpStatusCode204(string productName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateProductCommand
            {
                ProductName = productName
            };

            var content = this.ConvertRequestToStringContent(command);
            var response = await httpClient.PostAsync(this.ApiUrl, content);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var model = await this.ParseResponse<CreateProductResult>(response);

            model.ShouldNotBeNull();
            model.ProductId.ShouldNotBeNull().ShouldNotBe(ProductId.Empty);

            await this.ExecuteDbContextAsync(async dbContext =>
            {
                var product = await dbContext.Set<Product>().FirstOrDefaultAsync(_ => _.Id == model.ProductId);

                product.ShouldNotBeNull();
            });
        });
    }

    [Fact(DisplayName = "Create Product With Invalid Request Should Return HttpStatusCode400")]
    public async Task Create_Product_With_Invalid_Request_Should_Return_HttpStatusCode400()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateProductCommand();

            var content = this.ConvertRequestToStringContent(command);

            var response = await httpClient.PostAsync(this.ApiUrl, content);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        });
    }
}
