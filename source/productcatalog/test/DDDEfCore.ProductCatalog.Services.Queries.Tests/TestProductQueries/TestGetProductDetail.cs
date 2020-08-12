using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail;
using FluentValidation;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestProductQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetProductDetail : IClassFixture<TestProductsFixture>
    {
        private readonly TestProductsFixture _testProductsFixture;

        public TestGetProductDetail(TestProductsFixture testProductsFixture)
            => this._testProductsFixture = testProductsFixture;

        [Fact(DisplayName = "Should get ProductDetail Correctly")]
        public async Task Should_Get_ProductDetail_Correctly()
        {
            var product = this._testProductsFixture.Product;

            var request = new GetProductDetailRequest
            {
                ProductId = product.Id
            };

            await this._testProductsFixture.ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result =>
            {
                result.ShouldNotBeNull();

                var productDetail = result.Product;
                productDetail.ShouldNotBeNull();
                productDetail.Id.ShouldBe(product.Id);
                productDetail.Name.ShouldBe(product.Name);

                result.CatalogCategories.ShouldHaveSingleItem();
                var catalogCategory = result.CatalogCategories.FirstOrDefault();
                catalogCategory.CatalogCategoryId.ShouldBe(this._testProductsFixture.CatalogCategory.Id);
                catalogCategory.CatalogCategoryName.ShouldBe(this._testProductsFixture.CatalogCategory.DisplayName);
                catalogCategory.CatalogId.ShouldBe(this._testProductsFixture.Catalog.Id);
                catalogCategory.CatalogName.ShouldBe(this._testProductsFixture.Catalog.DisplayName);
                catalogCategory.ProductDisplayName.ShouldBe(this._testProductsFixture.CatalogProduct.DisplayName);
            });
        }

        [Fact(DisplayName = "Not Found Product Should Return Empty")]
        public async Task NotFound_Product_Should_Return_Empty()
        {
            var request = new GetProductDetailRequest {ProductId = ProductId.New};
            await this._testProductsFixture.ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result =>
            {
                result.ShouldNotBeNull(); 

                result.Product.ShouldNotBeNull();
                result.Product.Id.ShouldBeNull();
                result.Product.Name.ShouldBeNullOrWhiteSpace();

                result.CatalogCategories.ShouldBeEmpty();
            });
        }

        [Fact(DisplayName = "Invalid Request Should Throw ValidationException")]
        public async Task Invalid_Request_Should_Throw_ValidationException()
        {
            var request = new GetProductDetailRequest{ProductId = ProductId.Empty};

            await Should.ThrowAsync<ValidationException>(async ()=>
                await this._testProductsFixture
                    .ExecuteTestRequestHandler<GetProductDetailRequest, GetProductDetailResult>(request, result => { }));
        }

        [Fact(DisplayName = "Invalid Request Should Fail Validation")]
        public async Task Invalid_Request_Should_Fail_Validation()
        {
            var request = new GetProductDetailRequest { ProductId = ProductId.Empty };
            await this._testProductsFixture
                .ExecuteValidationTest(request, result => { result.ShouldHaveValidationErrorFor(x => x.ProductId); });
        }
    }
}
