using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using FluentValidation;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Frameworks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogCategoryQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogCategoryDetail : IClassFixture<TestCatalogCategoryFixture>
    {
        private readonly TestCatalogCategoryFixture _testFixture;

        public TestGetCatalogCategoryDetail(TestCatalogCategoryFixture testFixture)
            => this._testFixture = testFixture;

        [Theory(DisplayName = "Should GetCatalogCategoryDetail With Paging CatalogProduct Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Should_GetCatalogCategoryDetail_With_Paging_CatalogProduct_Correctly(int pageIndex, int pageSize)
        {
            var catalogCategory = this._testFixture.CatalogCategory;
            var catalogCategoryId = catalogCategory.CatalogCategoryId;
            var request = new GetCatalogCategoryDetailRequest
            {
                CatalogCategoryId = catalogCategoryId,
                CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };

            await this._testFixture.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>
            {
                result.ShouldNotBeNull();
                result.CatalogCategoryDetail.ShouldNotBeNull();
                result.CatalogCategoryDetail.CatalogId.ShouldBe(this._testFixture.Catalog.CatalogId);
                result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
                result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(this._testFixture.CatalogCategory.DisplayName);
                result.CatalogCategoryDetail.CatalogName.ShouldBe(this._testFixture.Catalog.DisplayName);

                result.TotalOfCatalogProducts.ShouldBe(catalogCategory.Products.Count());

                result.CatalogProducts.ToList().ForEach(c =>
                {
                    var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x.CatalogProductId == c.CatalogProductId);

                    catalogProduct.ShouldNotBeNull();
                    c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                    c.ProductId.ShouldBe(catalogProduct.ProductId);
                });
            });
        }

        [Fact(DisplayName = "Should GetCatalogCategoryDetail With Search CatalogProduct Correctly")]
        public async Task Should_GetCatalogCategoryDetail_With_Search_CatalogProduct_Correctly()
        {
            var catalogCategory = this._testFixture.CatalogCategory;
            var catalogCategoryId = catalogCategory.CatalogCategoryId;
            
            var request = new GetCatalogCategoryDetailRequest
            {
                CatalogCategoryId = catalogCategoryId,
                CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
                {
                    SearchTerm = this._testFixture.CatalogProduct.DisplayName
                }
            };

            await this._testFixture.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>
            {
                result.ShouldNotBeNull();
                result.CatalogCategoryDetail.ShouldNotBeNull();
                result.CatalogCategoryDetail.CatalogId.ShouldBe(catalogCategory.CatalogId);
                result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
                result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(catalogCategory.DisplayName);
                result.CatalogCategoryDetail.CatalogName.ShouldBe(this._testFixture.Catalog.DisplayName);

                result.TotalOfCatalogProducts.ShouldBe(1);

                result.CatalogProducts.ToList().ForEach(c =>
                {
                    var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x.CatalogProductId == c.CatalogProductId);

                    catalogProduct.ShouldNotBeNull();
                    c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                    c.ProductId.ShouldBe(catalogProduct.ProductId);
                });
            });
        }

        [Fact(DisplayName = "GetCatalogCategoryDetail With Invalid Request Should Throw Validation Exception")]
        public async Task GetCatalogCategoryDetail_With_Invalid_Request_ShouldThrow_ValidationException()
        {
            var request = new GetCatalogCategoryDetailRequest
            {
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty
            };

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._testFixture.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>{}));
        }

        [Fact(DisplayName = "Should Validate GetCatalogCategoryDetailRequest Correctly")]
        public async Task Should_Validate_GetCatalogCategoryDetailRequest_Correctly()
        {
            var request = new GetCatalogCategoryDetailRequest
            {
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty
            };

            await this._testFixture.ExecuteValidationTest(request,
                result => { result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId); });
        }
    }
}
