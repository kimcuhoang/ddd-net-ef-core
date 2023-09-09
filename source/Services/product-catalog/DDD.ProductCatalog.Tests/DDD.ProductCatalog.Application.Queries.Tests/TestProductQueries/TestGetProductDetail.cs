using DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestProductQueries;

public class TestGetProductDetail : TestProductQueriesBase
{
    public TestGetProductDetail(TestQueriesCollectionFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    [Fact(DisplayName = "Should get ProductDetail Correctly")]
    public async Task Should_Get_ProductDetail_Correctly()
    {
        var product = this.Product;

        var request = new GetProductDetailRequest
        {
            ProductId = product.Id
        };

        await this.ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();

            var productDetail = result.Product;
            productDetail.ShouldNotBeNull();
            productDetail.Id.ShouldBe(product.Id);
            productDetail.Name.ShouldBe(product.Name);

            result.CatalogCategories.ShouldHaveSingleItem();
            var catalogCategory = result.CatalogCategories.FirstOrDefault();
            catalogCategory.ShouldNotBeNull();
            catalogCategory.CatalogCategoryId.ShouldBe(this.CatalogCategory.Id);
            catalogCategory.CatalogCategoryName.ShouldBe(this.CatalogCategory.DisplayName);
            catalogCategory.CatalogId.ShouldBe(this.Catalog.Id);
            catalogCategory.CatalogName.ShouldBe(this.Catalog.DisplayName);
            catalogCategory.ProductDisplayName.ShouldBe(this.CatalogProduct.DisplayName);
        });
    }

    [Fact(DisplayName = "Not Found Product Should Return Empty")]
    public async Task NotFound_Product_Should_Return_Empty()
    {
        var request = new GetProductDetailRequest { ProductId = ProductId.New };
        await this.ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();

            result.Product.ShouldNotBeNull();
            result.Product.Id.ShouldBeNull();
            result.Product.Name.ShouldBeNullOrWhiteSpace();

            result.CatalogCategories.ShouldBeEmpty();
        });
    }

    [Fact(DisplayName = "Invalid Request Should Fail Validation")]
    public async Task Invalid_Request_Should_Fail_Validation()
    {
        var request = new GetProductDetailRequest { ProductId = ProductId.Empty };
        await this.ExecuteValidationTest(request, result => { result.ShouldHaveValidationErrorFor(x => x.ProductId); });
    }
}
