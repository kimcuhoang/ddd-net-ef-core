using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Net;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestGetProductDetail : TestBase<TestProductsControllerFixture>
{
    public TestGetProductDetail(ITestOutputHelper testOutput, TestProductsControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private Catalog Catalog => this._fixture.Catalog;
    private Product Product => this._fixture.Product;
    private CatalogCategory CatalogCategory => this._fixture.CatalogCategory;
    private CatalogProduct CatalogProduct => this._fixture.CatalogProduct;

    private string ApiUrl => $"{this._fixture.BaseUrl}/{(Guid) this.Product.Id}";

   

    [Fact(DisplayName = "Should GetProductDetail by ProductId Correctly")]
    public async Task Should_GetProductDetail_By_ProductId_Correctly()
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var response = await client.GetAsync(this.ApiUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var productDetailResult =
                JsonSerializer.Deserialize<GetProductDetailResult>(result, jsonSerializerOptions);

            productDetailResult.ShouldNotBeNull();

            var productDetail = productDetailResult.Product;
            productDetail.ShouldNotBeNull();
            productDetail.Id.ShouldBe(this.Product.Id);
            productDetail.Name.ShouldBe(this.Product.Name);

            var theirCategories = productDetailResult.CatalogCategories;
            theirCategories.ShouldHaveSingleItem();
            var catalogCategory = theirCategories.FirstOrDefault();
            catalogCategory.ShouldNotBeNull();
            catalogCategory.CatalogCategoryId.ShouldBe(this.CatalogCategory.Id);
            catalogCategory.CatalogCategoryName.ShouldBe(this.CatalogCategory.DisplayName);
            catalogCategory.CatalogId.ShouldBe(this.Catalog.Id);
            catalogCategory.CatalogName.ShouldBe(this.Catalog.DisplayName);
            catalogCategory.CatalogProductId.ShouldBe(this.CatalogProduct.Id);
            catalogCategory.ProductDisplayName.ShouldBe(this.CatalogProduct.DisplayName);
        });
    }

    [Fact(DisplayName = "Invalid ProductId Should Return HttpStatusCode400")]
    public async Task Invalid_ProductId_Should_Return_HttpStatusCode400()
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var apiUrl = $"{this._fixture.BaseUrl}/{Guid.Empty}";
            var response = await client.GetAsync(apiUrl);

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

    [Theory(DisplayName = "Not Found Product Should Return Empty Result With HttpStatusCode200")]
    [AutoData]
    public async Task NotFound_Product_Should_Return_Empty_Result_With_HttpStatusCode200(Guid randomProductId)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var apiUrl = $"{this._fixture.BaseUrl}/{randomProductId}";
            var response = await client.GetAsync(apiUrl);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var productDetailResult =
                JsonSerializer.Deserialize<GetProductDetailResult>(result, jsonSerializerOptions);

            productDetailResult.ShouldNotBeNull();
            var productDetail = productDetailResult.Product;
            productDetail.ShouldNotBeNull();
            productDetail.Id.ShouldBeNull();
            productDetail.Name.ShouldBeNullOrWhiteSpace();

            productDetailResult.CatalogCategories.ShouldBeEmpty();
        });
    }
}
