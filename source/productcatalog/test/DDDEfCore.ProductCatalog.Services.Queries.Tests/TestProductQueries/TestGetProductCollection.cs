using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestProductQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetProductCollection : IClassFixture<TestProductsFixture>
    {
        private readonly TestProductsFixture _testProductsFixture;

        public TestGetProductCollection(TestProductsFixture testProductsFixture)
            => this._testProductsFixture = testProductsFixture;

        [Theory(DisplayName = "Should GetProductCollection With Paging Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        public async Task Should_GetProductCollection_WithPaging_Correctly(int pageIndex, int pageSize)
        {
            var request = new GetProductCollectionRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await this._testProductsFixture.ExecuteTestRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalProducts.ShouldBe(1);
                result.Products.ShouldHaveSingleItem();

                var product = this._testProductsFixture.Product;
                var productResult = result.Products.FirstOrDefault();

                productResult.ShouldNotBeNull();
                productResult.Id.ShouldBe(product.ProductId);
                productResult.DisplayName.ShouldBe(product.Name);
            });
        }

        [Fact(DisplayName = "Search Products by Name and return correctly")]
        public async Task Search_Products_By_Name_And_Return_Correctly()
        {
            var product = this._testProductsFixture.Product;
            var request = new GetProductCollectionRequest
            {
                SearchTerm = product.Name,
                PageIndex = 1,
                PageSize = int.MaxValue
            };

            await this._testProductsFixture.ExecuteTestRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalProducts.ShouldBe(1);
                result.Products.ShouldHaveSingleItem();

                var productResult = result.Products.FirstOrDefault();

                productResult.ShouldNotBeNull();
                productResult.Id.ShouldBe(product.ProductId);
                productResult.DisplayName.ShouldBe(product.Name);
            });
        }

        [Fact(DisplayName = "Search And Not Found Any Products Should Return Empty")]
        public async Task Search_And_NotFound_Any_Products_Should_ReturnEmpty()
        {
            var request = GenFu.A.New<GetProductCollectionRequest>();

            await this._testProductsFixture.ExecuteTestRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalProducts.ShouldBe(0);
                result.Products.ShouldBeEmpty();
            });
        }
    }
}
