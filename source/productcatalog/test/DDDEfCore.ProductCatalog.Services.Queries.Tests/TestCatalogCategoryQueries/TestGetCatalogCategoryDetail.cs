using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using FluentValidation;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogCategoryQueries;

public class TestGetCatalogCategoryDetail : TestBase<TestCatalogCategoryFixture>
{
    public TestGetCatalogCategoryDetail(ITestOutputHelper testOutput, TestCatalogCategoryFixture fixture) 
        : base(testOutput, fixture)
    {
    }

    [Theory(DisplayName = "Should GetCatalogCategoryDetail With Paging CatalogProduct Correctly")]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task Should_GetCatalogCategoryDetail_With_Paging_CatalogProduct_Correctly(int pageIndex, int pageSize)
    {
        var catalogCategory = this._fixture.CatalogCategory;
        var catalogCategoryId = catalogCategory.Id;
        var request = new GetCatalogCategoryDetailRequest
        {
            CatalogCategoryId = catalogCategoryId,
            CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CatalogCategoryDetail.ShouldNotBeNull();
            result.CatalogCategoryDetail.CatalogId.ShouldBe(this._fixture.Catalog.Id);
            result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
            result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(this._fixture.CatalogCategory.DisplayName);
            result.CatalogCategoryDetail.CatalogName.ShouldBe(this._fixture.Catalog.DisplayName);

            result.TotalOfCatalogProducts.ShouldBe(catalogCategory.Products.Count());

            result.CatalogProducts.ToList().ForEach(c =>
            {
                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x.Id == c.CatalogProductId);

                catalogProduct.ShouldNotBeNull();
                c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                c.ProductId.ShouldBe(catalogProduct.ProductId);
            });
        });
    }

    [Fact(DisplayName = "Should GetCatalogCategoryDetail With Search CatalogProduct Correctly")]
    public async Task Should_GetCatalogCategoryDetail_With_Search_CatalogProduct_Correctly()
    {
        var catalogCategory = this._fixture.CatalogCategory;
        var catalogCategoryId = catalogCategory.Id;
        
        var request = new GetCatalogCategoryDetailRequest
        {
            CatalogCategoryId = catalogCategoryId,
            CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
            {
                SearchTerm = this._fixture.CatalogProduct.DisplayName
            }
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CatalogCategoryDetail.ShouldNotBeNull();
            result.CatalogCategoryDetail.CatalogId.ShouldBe(catalogCategory.CatalogId);
            result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
            result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(catalogCategory.DisplayName);
            result.CatalogCategoryDetail.CatalogName.ShouldBe(this._fixture.Catalog.DisplayName);

            result.TotalOfCatalogProducts.ShouldBe(1);

            result.CatalogProducts.ToList().ForEach(c =>
            {
                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x.Id == c.CatalogProductId);

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
            CatalogCategoryId = CatalogCategoryId.Empty
        };

        await Should.ThrowAsync<ValidationException>(async () =>
            await this._fixture.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>{}));
    }

    [Fact(DisplayName = "Should Validate GetCatalogCategoryDetailRequest Correctly")]
    public async Task Should_Validate_GetCatalogCategoryDetailRequest_Correctly()
    {
        var request = new GetCatalogCategoryDetailRequest
        {
            CatalogCategoryId = CatalogCategoryId.Empty
        };

        await this._fixture.ExecuteValidationTest(request,
            result => { result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId); });
    }
}
