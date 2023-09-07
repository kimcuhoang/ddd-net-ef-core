using DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;
using DDD.ProductCatalog.Core.Products;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestProductQueries;

public class TestGetProductDetail : TestBase<TestProductsFixture>
{
    public TestGetProductDetail(ITestOutputHelper testOutput, TestProductsFixture fixture) : base(testOutput, fixture)
    {
    }

    [Fact(DisplayName = "Should get ProductDetail Correctly")]
    public async Task Should_Get_ProductDetail_Correctly()
    {
        var product = this._fixture.Product;

        var request = new GetProductDetailRequest
        {
            ProductId = product.Id
        };

        await this._fixture.ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();

            var productDetail = result.Product;
            productDetail.ShouldNotBeNull();
            productDetail.Id.ShouldBe(product.Id);
            productDetail.Name.ShouldBe(product.Name);

            result.CatalogCategories.ShouldHaveSingleItem();
            var catalogCategory = result.CatalogCategories.FirstOrDefault();
            catalogCategory.ShouldNotBeNull();
            catalogCategory.CatalogCategoryId.ShouldBe(this._fixture.CatalogCategory.Id);
            catalogCategory.CatalogCategoryName.ShouldBe(this._fixture.CatalogCategory.DisplayName);
            catalogCategory.CatalogId.ShouldBe(this._fixture.Catalog.Id);
            catalogCategory.CatalogName.ShouldBe(this._fixture.Catalog.DisplayName);
            catalogCategory.ProductDisplayName.ShouldBe(this._fixture.CatalogProduct.DisplayName);
        });
    }

    [Fact(DisplayName = "Not Found Product Should Return Empty")]
    public async Task NotFound_Product_Should_Return_Empty()
    {
        var request = new GetProductDetailRequest { ProductId = ProductId.New };
        await this._fixture.ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result =>
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
        await this._fixture
            .ExecuteValidationTest(request, result => { result.ShouldHaveValidationErrorFor(x => x.ProductId); });
    }
}
