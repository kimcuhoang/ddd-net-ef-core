using DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestSearchProduct(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : TestProductsControllerBase(testCollectionFixture, output)
{
    private string ApiUrl => $"{this.BaseUrl}/search";

    private List<Product> Products => new() { this.Product };



    [Theory(DisplayName = "Search Categories With Paging Correctly")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    [InlineData(null, 1)]
    [InlineData(null, null)]
    public async Task Search_Categories_With_Paging_Correctly(int? pageIndex, int? pageSize)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("searchTerm", this.Product.Name);

            if (pageIndex.HasValue)
            {
                parameters.Add(nameof(pageIndex), $"{pageIndex.Value}");
            }

            if (pageSize.HasValue)
            {
                parameters.Add(nameof(pageSize), $"{pageSize.Value}");
            }

            var searchUrl = $"{this.ApiUrl}?{parameters}";

            var response = await httpClient.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var productsResult = await this.ParseResponse<GetProductCollectionResult>(response);

            productsResult.ShouldNotBeNull();
            productsResult.TotalProducts.ShouldBe(this.Products.Count);
            productsResult.Products.ToList().ForEach(product =>
            {
                var predefinedProduct = this.Products.SingleOrDefault(x =>
                    x.Id == product.Id && x.Name == product.DisplayName);
                predefinedProduct.ShouldNotBeNull();
            });
        });
    }

    [Theory(DisplayName = "Search not found still return HttpStatusCode200")]
    [AutoData]
    public async Task Search_Not_Found_Still_Return_HttpStatusCode200(string randomSearchTerm)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("searchTerm", randomSearchTerm);

            var searchUrl = $"{this.ApiUrl}?{parameters}";

            var response = await httpClient.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var productsResult = await this.ParseResponse<GetProductCollectionResult>(response);

            productsResult.ShouldNotBeNull();
            productsResult.TotalProducts.ShouldBe(0);
            productsResult.Products.ShouldBeEmpty();
        });
    }

    [Theory(DisplayName = "Invalid Search Request Should Return HttpStatusCode400")]
    [InlineData(1, 0)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    public async Task Invalid_Search_Request_Should_Return_HttpStatusCode400(int pageIndex, int pageSize)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add(nameof(pageIndex), $"{pageIndex}");
            parameters.Add(nameof(pageSize), $"{pageSize}");

            var searchUrl = $"{this.ApiUrl}?{parameters}";
            var response = await httpClient.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var searchResult = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);
            searchResult.ShouldNotBeNull();
            searchResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            searchResult.ErrorMessages.Count.ShouldBeGreaterThan(0);
        });
    }
}
