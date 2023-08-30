using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail;
using FluentValidation;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogProductQueries;

public class TestGetCatalogProductDetail : TestBase<TestCatalogProductFixture>
{
    public TestGetCatalogProductDetail(ITestOutputHelper testOutput, TestCatalogProductFixture fixture) : base(testOutput, fixture)
    {
    }

    [Fact(DisplayName = "GetCatalogProductDetail Correctly")]
    public async Task GetCatalogProductDetail_Correctly()
    {
        var catalogProduct = this._fixture.CatalogProduct;
        var catalogProductId = catalogProduct.Id;
        var catalogCategory = this._fixture.CatalogCategory;
        var catalogCategoryId = catalogCategory.Id;
        var catalog = this._fixture.Catalog;
        var catalogId = catalog.Id;

        var request = new GetCatalogProductDetailRequest
        {
            CatalogProductId = catalogProductId
        };
        await this._fixture.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>
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
            CatalogProductId = CatalogProductId.New
        };
        await this._fixture.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>
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
            CatalogProductId = CatalogProductId.Empty
        };

        await Should.ThrowAsync<ValidationException>(async () =>
            await this._fixture.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>{}));
    }

    [Fact(DisplayName = "Should Validate Request Correctly")]
    public async Task Should_Validate_Request_Correctly()
    {
        var request = new GetCatalogProductDetailRequest
        {
            CatalogProductId = CatalogProductId.Empty
        };

        await this._fixture.ExecuteValidationTest(request,
            result => { result.ShouldHaveValidationErrorFor(x => x.CatalogProductId); });
    }
}
