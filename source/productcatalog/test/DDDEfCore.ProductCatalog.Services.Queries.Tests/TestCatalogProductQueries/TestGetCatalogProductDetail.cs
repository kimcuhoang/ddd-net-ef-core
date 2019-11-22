using DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogProductQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogProductDetail : IClassFixture<TestCatalogProductFixture>
    {
        private readonly TestCatalogProductFixture _testFixture;

        public TestGetCatalogProductDetail(TestCatalogProductFixture testFixture)
        {
            this._testFixture = testFixture;
        }

        [Fact(DisplayName = "GetCatalogProductDetail Correctly")]
        public async Task GetCatalogProductDetail_Correctly()
        {
            var catalogProduct = this._testFixture.CatalogProduct;
            var catalogProductId = catalogProduct.CatalogProductId;
            var catalogCategory = this._testFixture.CatalogCategory;
            var catalogCategoryId = catalogCategory.CatalogCategoryId;
            var catalog = this._testFixture.Catalog;
            var catalogId = catalog.CatalogId;

            var request = new GetCatalogProductDetailRequest
            {
                CatalogProductId = catalogProductId
            };
            await this._testFixture.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>
            {
                result.ShouldNotBeNull();
                
                result.CatalogProduct.ShouldNotBeNull();
                result.CatalogProduct.CatalogProductId.ShouldBe(catalogProductId);
                result.CatalogProduct.DisplayName.ShouldBe(catalogProduct.DisplayName);

                result.CatalogCategory.ShouldNotBeNull();
                result.CatalogCategory.CatalogCategoryId.ShouldBe(catalogCategoryId);
                result.CatalogCategory.DisplayName.ShouldBe(catalogCategory.DisplayName);

                result.Catalog.ShouldNotBeNull();
                result.Catalog.CatalogId.ShouldBe(catalogId);
                result.Catalog.CatalogName.ShouldBe(catalog.DisplayName);
            });
        }

        [Fact(DisplayName = "GetCatalogProductDetail Not Found CatalogProduct Still Work Correctly")]
        public async Task GetCatalogProductDetail_NotFound_CatalogProduct_Still_Work_Correctly()
        {
            var request = new GetCatalogProductDetailRequest
            {
                CatalogProductId = IdentityFactory.Create<CatalogProductId>()
            };
            await this._testFixture.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>
            {
                result.ShouldNotBeNull();
                result.IsNull.ShouldBe(true);
                result.CatalogCategory.ShouldNotBeNull();
                result.CatalogCategory.CatalogCategoryId.ShouldBeNull();
                string.IsNullOrWhiteSpace(result.CatalogCategory.DisplayName).ShouldBeTrue();

                result.Catalog.ShouldNotBeNull();
                result.Catalog.CatalogId.ShouldBeNull();
                string.IsNullOrWhiteSpace(result.Catalog.CatalogName).ShouldBeTrue();
            });
        }

        [Fact(DisplayName = "Invalid Request Should Throw Validation Exception")]
        public async Task Invalid_Request_Should_Throw_ValidationException()
        {
            var request = new GetCatalogProductDetailRequest
            {
                CatalogProductId = (CatalogProductId)Guid.Empty
            };

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._testFixture.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>{}));
        }

        [Fact(DisplayName = "Should Validate Request Correctly")]
        public async Task Should_Validate_Request_Correctly()
        {
            var request = new GetCatalogProductDetailRequest
            {
                CatalogProductId = (CatalogProductId)Guid.Empty
            };

            await this._testFixture.ExecuteValidationTest(request,
                result => { result.ShouldHaveValidationErrorFor(x => x.CatalogProductId); });
        }
    }
}
