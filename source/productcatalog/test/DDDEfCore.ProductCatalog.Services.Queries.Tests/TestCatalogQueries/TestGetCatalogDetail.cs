using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail;
using FluentValidation;
using GenFu;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries;

public class TestGetCatalogDetail : TestBase<TestGetCatalogFixture>
{
    public TestGetCatalogDetail(ITestOutputHelper testOutput, TestGetCatalogFixture fixture) : base(testOutput, fixture)
    {
    }

    [Theory(DisplayName = "Should GetCatalogDetail With Paging CatalogCategory Correctly")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    public async Task Should_GetCatalogDetail_With_Paging_CatalogCategory_Correctly(int pageIndex, int pageSize)
    {
        var catalog = this._fixture.CatalogHasCatalogCategory;

        var request = new GetCatalogDetailRequest
        {
            CatalogId = catalog.Id,
            SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CatalogDetail.ShouldNotBeNull();
            result.CatalogDetail.Id.ShouldBe(catalog.Id);
            result.CatalogDetail.DisplayName.ShouldBe(catalog.DisplayName);
            
            result.TotalOfCatalogCategories.ShouldBe(catalog.Categories.Count());

            result.CatalogCategories.ToList().ForEach(c =>
            {
                var catalogCategory =
                    catalog.Categories.SingleOrDefault(x => x.Id == c.Id);
                catalogCategory.ShouldNotBeNull();
                c.CategoryId.ShouldBe(this._fixture.Category.Id);
                c.DisplayName.ShouldBe(catalogCategory.DisplayName);
                c.TotalOfProducts.ShouldBe(catalogCategory.Products.Count());
            });
        });
    }

    [Fact(DisplayName = "Should GetCatalogDetail With Search CatalogCategory Correctly")]
    public async Task Should_GetCatalogDetail_With_Search_CatalogCategory_Correctly()
    {
        var catalog = this._fixture.CatalogHasCatalogCategory;
        var catalogId = catalog.Id;
        var catalogCategories = catalog.Categories.ToList();

        var randomIndex = A.Random.Next(0, catalogCategories.Count);
        var randomCatalogCategory = catalogCategories[randomIndex];

        var request = new GetCatalogDetailRequest
        {
            CatalogId = catalog.Id,
            SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
            {
                SearchTerm = randomCatalogCategory.DisplayName
            }
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CatalogDetail.ShouldSatisfyAllConditions(
                () => result.CatalogDetail.Id.ShouldBe(catalogId),
                () => result.CatalogDetail.DisplayName.ShouldBe(catalog.DisplayName)
            );
            result.TotalOfCatalogCategories.ShouldBe(catalogCategories.Count());

            result.CatalogCategories.ToList().ForEach(c =>
            {
                var catalogCategory =
                    catalog.Categories.SingleOrDefault(x => x.Id == c.Id);

                catalogCategory.ShouldNotBeNull();
                c.CategoryId.ShouldBe(this._fixture.Category.Id);
                c.DisplayName.ShouldBe(catalogCategory.DisplayName);
                c.TotalOfProducts.ShouldBe(catalogCategory.Products.Count());
            });
        });
    }

    [Fact(DisplayName = "Should GetCatalogDetail With Not Found Any CatalogCategory From Search Still Correct")]
    public async Task Should_GetCatalogDetail_With_NotFound_Any_CatalogCategory_From_Search_Still_Correct()
    {
        var request = new GetCatalogDetailRequest { 
            CatalogId = this._fixture.CatalogWithoutCatalogCategory.Id
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.TotalOfCatalogCategories.ShouldBe(0);
            result.CatalogCategories.ShouldBeEmpty();
        });
    }

    [Theory(DisplayName = "Should Throw Exception When Request Is Invalid")]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task Should_Throw_ValidationException_When_Request_Is_Invalid(int pageIndex, int pageSize)
    {
        var request = new GetCatalogDetailRequest
        {
            CatalogId = CatalogId.Empty,
            SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }
        };

        await Should.ThrowAsync<ValidationException>(async () =>
            await this._fixture.ExecuteTestRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult>(request, result =>{}));
    }

    [Fact(DisplayName = "Should GetCatalogDetail Successfully Even Not Found Catalog")]
    public async Task Should_GetCatalogDetail_Successfully_Even_NotFound_Catalog()
    {
        var request = new GetCatalogDetailRequest { CatalogId = CatalogId.New };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.TotalOfCatalogCategories.ShouldBe(0);
            result.CatalogDetail.ShouldNotBeNull();
            result.CatalogDetail.Id.ShouldBeNull();
            result.CatalogDetail.DisplayName.ShouldBeNull();
            result.CatalogCategories.ShouldBeNull();
        });
    }

    [Theory(DisplayName = "Validation: GetCatalogDetail With Empty CatalogId Should Be Invalid")]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task GetCatalogDetail_With_Empty_CatalogId_ShouldBe_Invalid(int pageIndex, int pageSize)
    {
        var request = new GetCatalogDetailRequest
        {
            CatalogId = CatalogId.Empty,
            SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }
        };

        await this._fixture.ExecuteValidationTest(request, result =>
        {
            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            if (pageIndex < 0 || pageIndex == int.MaxValue)
            {
                result.ShouldHaveValidationErrorFor(x => x.SearchCatalogCategoryRequest.PageIndex);
            }

            if (pageSize < 0 || pageSize == int.MaxValue)
            {
                result.ShouldHaveValidationErrorFor(x => x.SearchCatalogCategoryRequest.PageSize);
            }
        });
    }
}
