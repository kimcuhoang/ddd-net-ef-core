using DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDD.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestGetProductDetail : TestProductsControllerBase
{
    public TestGetProductDetail(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    private string ApiUrl => $"{this.BaseUrl}/{(Guid)this.Product.Id}";



    [Fact(DisplayName = "Should GetProductDetail by ProductId Correctly")]
    public async Task Should_GetProductDetail_By_ProductId_Correctly()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var response = await httpClient.GetAsync(this.ApiUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);


            var productDetailResult = await this.ParseResponse<GetProductDetailResult>(response);

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
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var apiUrl = $"{this.BaseUrl}/{Guid.Empty}";
            var response = await httpClient.GetAsync(apiUrl);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var errorResponse = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }

    [Theory(DisplayName = "Not Found Product Should Return Empty Result With HttpStatusCode200")]
    [AutoData]
    public async Task NotFound_Product_Should_Return_Empty_Result_With_HttpStatusCode200(Guid randomProductId)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var apiUrl = $"{this.BaseUrl}/{randomProductId}";
            var response = await httpClient.GetAsync(apiUrl);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var productDetailResult = await this.ParseResponse<GetProductDetailResult>(response);

            productDetailResult.ShouldNotBeNull();
            var productDetail = productDetailResult.Product;
            productDetail.ShouldNotBeNull();
            productDetail.Id.ShouldBeNull();
            productDetail.Name.ShouldBeNullOrWhiteSpace();

            productDetailResult.CatalogCategories.ShouldBeEmpty();
        });
    }
}
