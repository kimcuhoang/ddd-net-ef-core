using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;
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

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var model = this._fixture.Parse<CreateProductResult>(result);

            model.ShouldNotBeNull();
            model.ProductId.ShouldNotBeNull().ShouldNotBe(ProductId.Empty);

            await this._fixture.ExecuteDbContextAsync(async dbContext =>
            {
                var product = await dbContext.Set<Product>().FirstOrDefaultAsync(_ => _.Id == model.ProductId);

                product.ShouldNotBeNull();
            });
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
        });
    }
}
