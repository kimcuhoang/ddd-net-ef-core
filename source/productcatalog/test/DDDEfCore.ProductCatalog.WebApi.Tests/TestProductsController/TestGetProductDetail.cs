using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController
{
    [Collection(nameof(SharedFixture))]
    public class TestGetProductDetail : IClassFixture<TestProductsControllerFixture>
    {
        private readonly TestProductsControllerFixture _testProductsControllerFixture;

        private Catalog Catalog => this._testProductsControllerFixture.Catalog;
        private Product Product => this._testProductsControllerFixture.Product;
        private CatalogCategory CatalogCategory => this._testProductsControllerFixture.CatalogCategory;
        private CatalogProduct CatalogProduct => this._testProductsControllerFixture.CatalogProduct;

        private string ApiUrl => $"{this._testProductsControllerFixture.BaseUrl}/{(Guid) this.Product.Id}";

        public TestGetProductDetail(TestProductsControllerFixture testProductsControllerFixture)
            => this._testProductsControllerFixture = testProductsControllerFixture;

        [Fact(DisplayName = "Should GetProductDetail by ProductId Correctly")]
        public async Task Should_GetProductDetail_By_ProductId_Correctly()
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var response = await client.GetAsync(this.ApiUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();
                var productDetailResult =
                    JsonSerializer.Deserialize<GetProductDetailResult>(result, jsonSerializationOptions);

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
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var apiUrl = $"{this._testProductsControllerFixture.BaseUrl}/{Guid.Empty}";
                var response = await client.GetAsync(apiUrl);

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

        [Theory(DisplayName = "Not Found Product Should Return Empty Result With HttpStatusCode200")]
        [AutoData]
        public async Task NotFound_Product_Should_Return_Empty_Result_With_HttpStatusCode200(Guid randomProductId)
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var apiUrl = $"{this._testProductsControllerFixture.BaseUrl}/{randomProductId}";
                var response = await client.GetAsync(apiUrl);

                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();
                var productDetailResult =
                    JsonSerializer.Deserialize<GetProductDetailResult>(result, jsonSerializationOptions);

                productDetailResult.ShouldNotBeNull();
                var productDetail = productDetailResult.Product;
                productDetail.ShouldNotBeNull();
                productDetail.Id.ShouldBeNull();
                productDetail.Name.ShouldBeNullOrWhiteSpace();

                productDetailResult.CatalogCategories.ShouldBeEmpty();
            });
        }
    }
}
