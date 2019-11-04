using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using FluentValidation;
using FluentValidation.TestHelper;
using GenFu;
using MediatR;
using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogCategoryQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogCategoryDetail : IClassFixture<TestCatalogCategoryFixture>
    {
        private readonly TestCatalogCategoryFixture _testFixture;
        private readonly GetCatalogCategoryDetailRequestValidator _validator;
        private readonly CancellationToken _cancellationToken;

        public TestGetCatalogCategoryDetail(TestCatalogCategoryFixture testFixture)
        {
            this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));
            this._validator = new GetCatalogCategoryDetailRequestValidator();
            this._cancellationToken = new CancellationToken(false);
        }

        private async Task ExecuteTestAndAssert(GetCatalogCategoryDetailRequest request, Action<GetCatalogCategoryDetailResult> assertAction)
        {
            await this._testFixture.ExecuteScopeAsync(async dbConnectionFactory =>
            {
                IRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult> requestHandler =
                    new RequestHandler(dbConnectionFactory, this._validator);

                var result = await requestHandler.Handle(request, this._cancellationToken);

                assertAction(result);
            });
        }

        [Theory(DisplayName = "Should GetCatalogCategoryDetail With Paging CatalogProduct Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Should_GetCatalogCategoryDetail_With_Paging_CatalogProduct_Correctly(int pageIndex, int pageSize)
        {
            var catalogCategory = this._testFixture.CatalogCategory;
            var catalogCategoryId = catalogCategory.CatalogCategoryId.Id;
            var request = new GetCatalogCategoryDetailRequest(catalogCategoryId)
            {
                CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.CatalogCategoryDetail.ShouldNotBeNull();
                result.CatalogCategoryDetail.CatalogId.ShouldBe(this._testFixture.Catalog.CatalogId.Id);
                result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
                result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(this._testFixture.CatalogCategory.DisplayName);
                result.CatalogCategoryDetail.CatalogName.ShouldBe(this._testFixture.Catalog.DisplayName);

                result.TotalOfCatalogProducts.ShouldBe(catalogCategory.Products.Count());

                result.CatalogProducts.ToList().ForEach(c =>
                {
                    var catalogProduct = catalogCategory.Products.SingleOrDefault(x =>
                        x.CatalogProductId == new CatalogProductId(c.CatalogProductId));

                    catalogProduct.ShouldNotBeNull();
                    c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                    c.ProductId.ShouldBe(catalogProduct.CatalogProductId.Id);
                });
            });
        }

        [Fact(DisplayName = "Should GetCatalogCategoryDetail With Search CatalogProduct Correctly")]
        public async Task Should_GetCatalogCategoryDetail_With_Search_CatalogProduct_Correctly()
        {
            var catalogCategory = this._testFixture.CatalogCategory;
            var catalogCategoryId = catalogCategory.CatalogCategoryId.Id;
            
            var randomIdx = A.Random.Next(0, this._testFixture.CatalogProducts.Count());
            var randomCatalogProduct = this._testFixture.CatalogProducts[randomIdx];

            var request = new GetCatalogCategoryDetailRequest(catalogCategoryId)
            {
                CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
                {
                    SearchTerm = randomCatalogProduct.DisplayName
                }
            };

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.CatalogCategoryDetail.ShouldNotBeNull();
                result.CatalogCategoryDetail.CatalogId.ShouldBe(catalogCategory.CatalogId.Id);
                result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
                result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(catalogCategory.DisplayName);
                result.CatalogCategoryDetail.CatalogName.ShouldBe(this._testFixture.Catalog.DisplayName);

                result.TotalOfCatalogProducts.ShouldBe(1);

                result.CatalogProducts.ToList().ForEach(c =>
                {
                    var catalogProduct = catalogCategory.Products.SingleOrDefault(x =>
                        x.CatalogProductId == new CatalogProductId(c.CatalogProductId));

                    catalogProduct.ShouldNotBeNull();
                    c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                    c.ProductId.ShouldBe(catalogProduct.CatalogProductId.Id);
                });
            });
        }

        [Fact(DisplayName = "GetCatalogCategoryDetail With Invalid Request Should Throw Validation Exception")]
        public async Task GetCatalogCategoryDetail_With_Invalid_Request_ShouldThrow_ValidationException()
        {
            var request = new GetCatalogCategoryDetailRequest(Guid.Empty);

            await this._testFixture.ExecuteScopeAsync(async dbConnectionFactory =>
            {
                IRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult> requestHandler
                    = new RequestHandler(dbConnectionFactory, this._validator);
                await Should.ThrowAsync<ValidationException>(async () =>
                    await requestHandler.Handle(request, this._cancellationToken));
            });
        }

        [Fact(DisplayName = "Should Validate GetCatalogCategoryDetailRequest Correctly")]
        public void Should_Validate_GetCatalogCategoryDetailRequest_Correctly()
        {
            var request = new GetCatalogCategoryDetailRequest(Guid.Empty);
            var result = this._validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        }
    }
}
