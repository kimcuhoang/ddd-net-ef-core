using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController
{
    [Collection(nameof(SharedFixture))]
    public class TestSearchCatalog : IClassFixture<TestCatalogsControllerFixture>
    {
        private readonly TestCatalogsControllerFixture _testCatalogsControllerFixture;

        private Catalog Catalog => this._testCatalogsControllerFixture.Catalog;

        private string ApiUrl => $"{this._testCatalogsControllerFixture.BaseUrl}/search";

        public TestSearchCatalog(TestCatalogsControllerFixture testCatalogsControllerFixture)
            => this._testCatalogsControllerFixture = testCatalogsControllerFixture;

        [Theory(DisplayName = "Search All With Paging Successfully")]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, null)]
        [InlineData(null, 1)]
        [InlineData(null, null)]
        public async Task Search_All_With_Paging_Successfully(int? pageIndex, int? pageSize)
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
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

                var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

                var response = await client.GetAsync(searchUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();

                var searchCatalogResult =
                    JsonSerializer.Deserialize<GetCatalogCollectionResult>(result, jsonSerializationOptions);

                searchCatalogResult.ShouldNotBeNull();
                searchCatalogResult.TotalCatalogs.ShouldBe(1);
                searchCatalogResult.CatalogItems.ShouldHaveSingleItem();

                var catalogItem = searchCatalogResult.CatalogItems.FirstOrDefault();
                catalogItem.ShouldNotBeNull();
                catalogItem.CatalogId.ShouldBe(this.Catalog.CatalogId);
                catalogItem.DisplayName.ShouldBe(this.Catalog.DisplayName);
                catalogItem.TotalCategories.ShouldBe(this.Catalog.Categories.Count());
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
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
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

                var response = await client.GetAsync(searchUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();

                var searchCatalogResult =
                    JsonSerializer.Deserialize<GetCatalogCollectionResult>(result, jsonSerializationOptions);

                searchCatalogResult.ShouldNotBeNull();
                searchCatalogResult.TotalCatalogs.ShouldBe(1);
                searchCatalogResult.CatalogItems.ShouldHaveSingleItem();

                var catalogItem = searchCatalogResult.CatalogItems.FirstOrDefault();
                catalogItem.ShouldNotBeNull();
                catalogItem.CatalogId.ShouldBe(this.Catalog.CatalogId);
                catalogItem.DisplayName.ShouldBe(this.Catalog.DisplayName);
                catalogItem.TotalCategories.ShouldBe(this.Catalog.Categories.Count());
            });
        }

        [Theory(DisplayName = "Search not found still return HttpStatusCode200")]
        [AutoData]
        public async Task Search_Not_Found_Still_Return_HttpStatusCode200(string randomSearchTerm)
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
            {
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters.Add("searchTerm", randomSearchTerm);

                var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

                var response = await client.GetAsync(searchUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();
                var productsResult =
                    JsonSerializer.Deserialize<GetCatalogCollectionResult>(result, jsonSerializationOptions);

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
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
            {
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters.Add(nameof(pageIndex), $"{pageIndex}");
                parameters.Add(nameof(pageSize), $"{pageSize}");

                var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";
                var response = await client.GetAsync(searchUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

                var result = await response.Content.ReadAsStringAsync();
                var searchResult =
                    JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result, jsonSerializationOptions);
                searchResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
                searchResult.ErrorMessages.Count.ShouldBeGreaterThan(0);
            });
        }
    }
}
