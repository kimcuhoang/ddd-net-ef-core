using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogDetail : IClassFixture<TestCatalogsControllerFixture>
    {
        private readonly TestCatalogsControllerFixture _testCatalogsControllerFixture;

        private Catalog Catalog => this._testCatalogsControllerFixture.Catalog;
        private CatalogCategory CatalogCategory => this._testCatalogsControllerFixture.CatalogCategory;
        private string ApiUrl => $"{this._testCatalogsControllerFixture.BaseUrl}";

        public TestGetCatalogDetail(TestCatalogsControllerFixture testCatalogsControllerFixture)
            => this._testCatalogsControllerFixture = testCatalogsControllerFixture;

        [Fact(DisplayName = "Get CatalogDetail Successfully")]
        public async Task Get_CatalogDetail_Successfully()
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var request = new GetCatalogDetailRequest
                {
                    CatalogId = this.Catalog.CatalogId
                };

                var content = request.ToStringContent(jsonSerializationOptions);
                var response = await client.PostAsync(this.ApiUrl, content);

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                var result = await response.Content.ReadAsStringAsync();
                var catalogDetailResult =
                    JsonSerializer.Deserialize<GetCatalogDetailResult>(result, jsonSerializationOptions);

                catalogDetailResult.ShouldNotBeNull();
                catalogDetailResult.TotalOfCatalogCategories.ShouldBe(this.Catalog.Categories.Count());

                var catalogDetail = catalogDetailResult.CatalogDetail;
                catalogDetail.ShouldNotBeNull();
                catalogDetail.Id.ShouldBe(this.Catalog.CatalogId);
                catalogDetail.DisplayName.ShouldBe(this.Catalog.DisplayName);

                catalogDetailResult.CatalogCategories.ToList().ForEach(category =>
                {
                    var catalogCategory = this.Catalog.Categories.SingleOrDefault(x =>
                        x.CatalogCategoryId == category.CatalogCategoryId
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
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var request = new GetCatalogDetailRequest
                {
                    CatalogId = this.Catalog.CatalogId,
                    SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
                    {
                        SearchTerm = this.CatalogCategory.DisplayName
                    }
                };

                var content = request.ToStringContent(jsonSerializationOptions);
                var response = await client.PostAsync(this.ApiUrl, content);

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                var result = await response.Content.ReadAsStringAsync();
                var catalogDetailResult =
                    JsonSerializer.Deserialize<GetCatalogDetailResult>(result, jsonSerializationOptions);

                catalogDetailResult.ShouldNotBeNull();
                catalogDetailResult.TotalOfCatalogCategories.ShouldBe(this.Catalog.Categories.Count());

                var catalogDetail = catalogDetailResult.CatalogDetail;
                catalogDetail.ShouldNotBeNull();
                catalogDetail.Id.ShouldBe(this.Catalog.CatalogId);
                catalogDetail.DisplayName.ShouldBe(this.Catalog.DisplayName);

                catalogDetailResult.CatalogCategories.ToList().ForEach(category =>
                {
                    var catalogCategory = this.Catalog.Categories.SingleOrDefault(x =>
                        x.CatalogCategoryId == category.CatalogCategoryId
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
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var request = new GetCatalogDetailRequest
                {
                    CatalogId = IdentityFactory.Create<CatalogId>(Guid.Empty),
                    SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
                    {
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    }
                };
                var content = request.ToStringContent(jsonSerializationOptions);
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

                var result = await response.Content.ReadAsStringAsync();
                var errorResponse =
                    JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                        jsonSerializationOptions);

                errorResponse.ShouldNotBeNull();
                errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
                errorResponse.ErrorMessages.ShouldNotBeEmpty();
            });
        }

        [Fact(DisplayName = "Not Found Catalog Should Return Empty Result")]
        public async Task NotFound_Catalog_Should_Return_Empty_Result()
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var request = new GetCatalogDetailRequest
                {
                    CatalogId = IdentityFactory.Create<CatalogId>(Guid.NewGuid())
                };
                var content = request.ToStringContent(jsonSerializationOptions);
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();
                var catalogDetailResult =
                    JsonSerializer.Deserialize<GetCatalogDetailResult>(result, jsonSerializationOptions);

                catalogDetailResult.ShouldNotBeNull();
                catalogDetailResult.CatalogDetail.ShouldNotBeNull();
                catalogDetailResult.CatalogDetail.Id.ShouldBeNull();
                catalogDetailResult.CatalogDetail.DisplayName.ShouldBeNullOrWhiteSpace();
                catalogDetailResult.CatalogCategories.ShouldBeNull();
            });
        }
    }
}
