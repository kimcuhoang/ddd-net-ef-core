using DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogProductQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogProductDetail : IClassFixture<TestCatalogProductFixture>
    {
        private readonly TestCatalogProductFixture _testFixture;
        private readonly GetCatalogProductDetailRequestValidator _validator;
        private readonly CancellationToken _cancellationToken;

        public TestGetCatalogProductDetail(TestCatalogProductFixture testFixture)
        {
            this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));
            this._validator = new GetCatalogProductDetailRequestValidator();
            this._cancellationToken = new CancellationToken(false);
        }

        private async Task ExecuteTestAndAssert(GetCatalogProductDetailRequest request, Action<GetCatalogProductDetailResult> assertFor)
        {
            await this._testFixture.ExecuteScopeAsync(async dbConnectionFactory =>
            {
                IRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult> requestHandler =
                    new RequestHandler(dbConnectionFactory, this._validator);

                var result = await requestHandler.Handle(request, this._cancellationToken);

                assertFor(result);
            });
        }

        [Fact(DisplayName = "GetCatalogProductDetail Correctly")]
        public async Task GetCatalogProductDetail_Correctly()
        {
            var catalogProduct = this._testFixture.CatalogProduct;
            var catalogProductId = catalogProduct.CatalogProductId.Id;
            var catalogCategory = this._testFixture.CatalogCategory;
            var catalogCategoryId = catalogCategory.CatalogCategoryId.Id;
            var catalog = this._testFixture.Catalog;
            var catalogId = catalog.CatalogId.Id;

            var request = new GetCatalogProductDetailRequest(catalogProductId);

            await this.ExecuteTestAndAssert(request, result =>
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
            var request = new GetCatalogProductDetailRequest(Guid.NewGuid());
            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.IsNull.ShouldBe(true);
                result.CatalogCategory.ShouldNotBeNull();
                result.CatalogCategory.CatalogCategoryId.ShouldBe(Guid.Empty);
                string.IsNullOrWhiteSpace(result.CatalogCategory.DisplayName).ShouldBeTrue();

                result.Catalog.ShouldNotBeNull();
                result.Catalog.CatalogId.ShouldBe(Guid.Empty);
                string.IsNullOrWhiteSpace(result.Catalog.CatalogName).ShouldBeTrue();
            });
        }

        [Fact(DisplayName = "Invalid Request Should Throw Validation Exception")]
        public async Task Invalid_Request_Should_Throw_ValidationException()
        {
            var request = new GetCatalogProductDetailRequest(Guid.Empty);

            await this._testFixture.ExecuteScopeAsync(async dbConnectionFactory =>
            {
                IRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult> requestHandler =
                    new RequestHandler(dbConnectionFactory, this._validator);

                await Should.ThrowAsync<ValidationException>(async () =>
                    await requestHandler.Handle(request, this._cancellationToken));
            });
        }

        [Fact(DisplayName = "Should Validate Request Correctly")]
        public void Should_Validate_Request_Correctly()
        {
            var request = new GetCatalogProductDetailRequest(Guid.Empty);
            var result = this._validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
        }
    }
}
