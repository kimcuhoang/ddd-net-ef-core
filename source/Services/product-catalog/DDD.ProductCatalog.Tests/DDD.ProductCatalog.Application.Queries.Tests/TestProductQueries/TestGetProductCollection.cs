using DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestProductQueries;

public class TestGetProductCollection : TestProductQueriesBase
{
    public TestGetProductCollection(TestQueriesCollectionFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    [Theory(DisplayName = "Should GetProductCollection With Paging Correctly")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    public async Task Should_GetProductCollection_WithPaging_Correctly(int pageIndex, int pageSize)
    {
        var request = new GetProductCollectionRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        await this.ExecuteTestRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalProducts.ShouldBeGreaterThanOrEqualTo(1);
            result.Products.ShouldNotBeEmpty();
        });
    }

    [Fact(DisplayName = "Search Products by Name and return correctly")]
    public async Task Search_Products_By_Name_And_Return_Correctly()
    {
        var product = this.Product;
        var request = new GetProductCollectionRequest
        {
            SearchTerm = product.Name
        };

        await this.ExecuteTestRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalProducts.ShouldBeGreaterThanOrEqualTo(1);
            result.Products.ShouldNotBeEmpty();

            var productResult = result.Products.FirstOrDefault();

            productResult.ShouldNotBeNull();
            productResult.Id.ShouldBe(product.Id);
            productResult.DisplayName.ShouldBe(product.Name);
        });
    }

    [Theory(DisplayName = "Should Validate Search Request Correctly")]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task Should_Validate_Search_Request_Correctly(int pageIndex, int pageSize)
    {
        var request = new GetProductCollectionRequest
        {
            PageSize = pageSize,
            PageIndex = pageIndex
        };

        await this.ExecuteValidationTest(request, result =>
        {
            if (pageIndex < 0 || pageIndex == int.MaxValue)
            {
                result.ShouldHaveValidationErrorFor(x => x.PageIndex);
            }

            if (pageSize < 0 || pageSize == int.MaxValue)
            {
                result.ShouldHaveValidationErrorFor(x => x.PageSize);
            }
        });
    }

    [Fact(DisplayName = "Search And Not Found Any Products Should Return Empty")]
    public async Task Search_And_NotFound_Any_Products_Should_ReturnEmpty()
    {
        var request = GenFu.GenFu.New<GetProductCollectionRequest>();

        await this.ExecuteTestRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalProducts.ShouldBe(0);
            result.Products.ShouldBeEmpty();
        });
    }
}
