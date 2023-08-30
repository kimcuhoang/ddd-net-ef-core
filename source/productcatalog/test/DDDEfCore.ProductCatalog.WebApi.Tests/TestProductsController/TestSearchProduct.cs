using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.JsonConverters;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController;

public class TestSearchProduct : TestBase<TestProductsControllerFixture>
{
    public TestSearchProduct(ITestOutputHelper testOutput, TestProductsControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private string ApiUrl => $"{this._fixture.BaseUrl}/search";

    private Product Product => this._fixture.Product;

    private List<Product> Products => new List<Product> {this.Product};

    

    [Theory(DisplayName = "Search Categories With Paging Correctly")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    [InlineData(null, 1)]
    [InlineData(null, null)]
    public async Task Search_Categories_With_Paging_Correctly(int? pageIndex, int? pageSize)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
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

            var response = await client.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();

            this._testOutput.WriteLine(result);

            var productsResult =
                JsonSerializer.Deserialize<GetProductCollectionResult>(result, jsonSerializerOptions);

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
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("searchTerm", randomSearchTerm);

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

            var response = await client.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var productsResult =
                JsonSerializer.Deserialize<GetProductCollectionResult>(result, jsonSerializerOptions);

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
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add(nameof(pageIndex), $"{pageIndex}");
            parameters.Add(nameof(pageSize), $"{pageSize}");

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";
            var response = await client.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();
            var searchResult =
                JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result, jsonSerializerOptions);
            searchResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            searchResult.ErrorMessages.Count.ShouldBeGreaterThan(0);
        });
    }
}
