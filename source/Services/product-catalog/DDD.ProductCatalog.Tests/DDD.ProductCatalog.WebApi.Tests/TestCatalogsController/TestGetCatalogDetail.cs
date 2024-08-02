using DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogDetail;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestGetCatalogDetail(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : TestCatalogsControllerBase(testCollectionFixture, output)
{
    private string ApiUrl => $"{this.BaseUrl}";


    [Fact(DisplayName = "Get CatalogDetail Successfully")]
    public async Task Get_CatalogDetail_Successfully()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var request = new GetCatalogDetailRequest
            {
                CatalogId = this.Catalog.Id,
                SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest()
            };

            var content = this.ConvertRequestToStringContent(request);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            var catalogDetailResult = await this.ParseResponse<GetCatalogDetailResult>(response);

            catalogDetailResult.ShouldNotBeNull();
            catalogDetailResult.TotalOfCatalogCategories.ShouldBe(this.Catalog.Categories.Count());

            var catalogDetail = catalogDetailResult.CatalogDetail;
            catalogDetail.ShouldNotBeNull();
            catalogDetail.Id.ShouldBe(this.Catalog.Id);
            catalogDetail.DisplayName.ShouldBe(this.Catalog.DisplayName);

            catalogDetailResult.CatalogCategories.ToList().ForEach(category =>
            {
                var catalogCategory = this.Catalog.Categories.SingleOrDefault(x =>
                    x.Id == category.Id
                    && category.DisplayName == x.DisplayName
                    && category.CategoryId == x.CategoryId);
                catalogCategory.ShouldNotBeNull();
                category.TotalOfProducts.ShouldBe(catalogCategory.Products.Count());
            });
        });
    }

    [Fact(DisplayName = "Get CatalogDetail Within Search CatalogCategory Successfully")]
    public async Task Get_CatalogDetail_Within_Search_CatalogCategory_Successfully()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var request = new GetCatalogDetailRequest
            {
                CatalogId = this.Catalog.Id,
                SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
                {
                    SearchTerm = this.CatalogCategory.DisplayName
                }
            };

            var content = this.ConvertRequestToStringContent(request);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            var catalogDetailResult = await this.ParseResponse<GetCatalogDetailResult>(response);

            catalogDetailResult.ShouldNotBeNull();
            catalogDetailResult.TotalOfCatalogCategories.ShouldBe(this.Catalog.Categories.Count());

            var catalogDetail = catalogDetailResult.CatalogDetail;
            catalogDetail.ShouldNotBeNull();
            catalogDetail.Id.ShouldBe(this.Catalog.Id);
            catalogDetail.DisplayName.ShouldBe(this.Catalog.DisplayName);

            catalogDetailResult.CatalogCategories.ToList().ForEach(category =>
            {
                var catalogCategory = this.Catalog.Categories.SingleOrDefault(x =>
                    x.Id == category.Id
                    && category.DisplayName == x.DisplayName
                    && category.CategoryId == x.CategoryId);
                catalogCategory.ShouldNotBeNull();
                category.TotalOfProducts.ShouldBe(catalogCategory.Products.Count());
            });
        });
    }

    [Theory(DisplayName = "Invalid GetCatalogDetail Request Should Return HttpStatusCode400")]
    [InlineData(1, 0)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    public async Task Invalid_GetCatalogDetail_Request_Should_Return_HttpStatusCode400(int pageIndex, int pageSize)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var request = new GetCatalogDetailRequest
            {
                CatalogId = CatalogId.Empty,
                SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };
            var content = this.ConvertRequestToStringContent(request);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            var errorResponse = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }

    [Fact(DisplayName = "Not Found Catalog Should Return Empty Result")]
    public async Task NotFound_Catalog_Should_Return_Empty_Result()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var request = new GetCatalogDetailRequest
            {
                CatalogId = CatalogId.New
            };
            var content = this.ConvertRequestToStringContent(request);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            var catalogDetailResult = await this.ParseResponse<GetCatalogDetailResult>(response);

            catalogDetailResult.ShouldNotBeNull();
            catalogDetailResult.CatalogDetail.ShouldNotBeNull();
            catalogDetailResult.CatalogDetail.Id.ShouldBeNull();
            catalogDetailResult.CatalogDetail.DisplayName.ShouldBeNullOrWhiteSpace();
            catalogDetailResult.CatalogCategories.ShouldBeNull();
        });
    }
}
