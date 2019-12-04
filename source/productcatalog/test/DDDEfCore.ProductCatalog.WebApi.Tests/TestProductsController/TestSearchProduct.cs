using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController
{
    [Collection(nameof(SharedFixture))]
    public class TestSearchProduct : IClassFixture<TestProductsControllerFixture>
    {
        private readonly TestProductsControllerFixture _testProductsControllerFixture;

        private string ApiUrl => $"{this._testProductsControllerFixture.BaseUrl}/search";

        private Product Product => this._testProductsControllerFixture.Product;

        private List<Product> Products => new List<Product> {this.Product};

        public TestSearchProduct(TestProductsControllerFixture testProductsControllerFixture)
            => this._testProductsControllerFixture = testProductsControllerFixture;

        [Theory(DisplayName = "Search Categories With Paging Correctly")]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, null)]
        [InlineData(null, 1)]
        [InlineData(null, null)]
        public async Task Search_Categories_With_Paging_Correctly(int? pageIndex, int? pageSize)
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
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

                var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

                var response = await client.GetAsync(searchUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();
                var productsResult =
                    JsonSerializer.Deserialize<GetProductCollectionResult>(result, jsonSerializationOptions);

                productsResult.ShouldNotBeNull();
                productsResult.TotalProducts.ShouldBe(this.Products.Count);
                productsResult.Products.ToList().ForEach(product =>
                {
                    var predefinedProduct = this.Products.SingleOrDefault(x =>
                        x.ProductId == product.Id && x.Name == product.DisplayName);
                    predefinedProduct.ShouldNotBeNull();
                });
            });
        }
        [Theory(DisplayName = "Search not found still return HttpStatusCode200")]
        [AutoData]
        public async Task Search_Not_Found_Still_Return_HttpStatusCode200(string randomSearchTerm)
        {
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters.Add("searchTerm", randomSearchTerm);

                var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

                var response = await client.GetAsync(searchUrl);
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                var result = await response.Content.ReadAsStringAsync();
                var productsResult =
                    JsonSerializer.Deserialize<GetProductCollectionResult>(result, jsonSerializationOptions);

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
            await this._testProductsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
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
