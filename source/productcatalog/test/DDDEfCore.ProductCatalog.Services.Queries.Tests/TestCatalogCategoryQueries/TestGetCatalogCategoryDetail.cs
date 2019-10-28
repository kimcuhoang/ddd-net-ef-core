using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using MediatR;
using Shouldly;
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

        private async Task DoAction(GetCatalogCategoryDetailRequest request, Action<GetCatalogCategoryDetailResult> assertAction)
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

            await this.DoAction(request, result =>
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
    }
}
