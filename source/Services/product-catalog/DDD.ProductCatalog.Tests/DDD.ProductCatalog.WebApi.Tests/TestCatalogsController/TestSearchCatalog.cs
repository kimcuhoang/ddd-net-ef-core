using DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogCollections;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDD.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestSearchCatalog : TestCatalogsControllerBase
{
    public TestSearchCatalog(WebApiTestFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    private string ApiUrl => $"{this.BaseUrl}/search";

    [Theory(DisplayName = "Search All With Paging Successfully")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    [InlineData(null, 1)]
    [InlineData(null, null)]
    public async Task Search_All_With_Paging_Successfully(int? pageIndex, int? pageSize)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);

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
            var searchCatalogResult = await this.ParseResponse<GetCatalogCollectionResult>(response);

            searchCatalogResult.ShouldNotBeNull();
            searchCatalogResult.TotalCatalogs.ShouldBeGreaterThanOrEqualTo(1);
            searchCatalogResult.CatalogItems.ShouldNotBeEmpty();
        });
    }

    [Theory(DisplayName = "Search With SearchTerm Successfully")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    [InlineData(null, 1)]
    [InlineData(null, null)]
    public async Task Search_With_SearchTerm_Successfully(int? pageIndex, int? pageSize)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("searchTerm", this.Catalog.DisplayName);
            if (pageIndex.HasValue)
            {
                parameters.Add(nameof(pageIndex), $"{pageIndex.Value}");
            }

            if (pageSize.HasValue)
            {
                parameters.Add(nameof(pageSize), $"{pageSize.Value}");
            }

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

            var response = await httpClient.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var searchCatalogResult = await this.ParseResponse<GetCatalogCollectionResult>(response);

            searchCatalogResult.ShouldNotBeNull();
            searchCatalogResult.TotalCatalogs.ShouldBe(1);
            searchCatalogResult.CatalogItems.ShouldHaveSingleItem();

            var catalogItem = searchCatalogResult.CatalogItems.FirstOrDefault();
            catalogItem.ShouldNotBeNull();
            catalogItem.CatalogId.ShouldBe(this.Catalog.Id);
            catalogItem.DisplayName.ShouldBe(this.Catalog.DisplayName);
            catalogItem.TotalCategories.ShouldBe(this.Catalog.Categories.Count());
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

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

            var response = await httpClient.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var productsResult = await this.ParseResponse<GetCatalogCollectionResult>(response);

            productsResult.ShouldNotBeNull();
            productsResult.TotalCatalogs.ShouldBe(0);
            productsResult.CatalogItems.ShouldBeEmpty();
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

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";
            var response = await httpClient.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var searchResult = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);
            searchResult.ShouldNotBeNull();
            searchResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            searchResult.ErrorMessages.Count.ShouldBeGreaterThan(0);
        });
    }
}
