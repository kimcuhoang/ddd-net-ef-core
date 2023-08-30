using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections;
using FluentValidation;
using GenFu;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries;

public class TestGetCatalogCollection : TestBase<TestGetCatalogFixture>
{
    public TestGetCatalogCollection(ITestOutputHelper testOutput, TestGetCatalogFixture fixture) : base(testOutput, fixture)
    {
    }

    [Theory(DisplayName = "Should GetCatalogCollection With Paging Correctly")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    public async Task Should_GetCatalogCollection_WithPaging_Correctly(int pageIndex, int pageSize)
    {
        var catalogs = this._fixture.Catalogs.ToList();
        var request = new GetCatalogCollectionRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalCatalogs.ShouldBeGreaterThanOrEqualTo(1);
            result.CatalogItems.ShouldNotBeEmpty();
        });
    }

    [Theory(DisplayName = "Invalid Search Request Should Throw ValidationException")]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task Invalid_Search_Request_Should_Throw_ValidationException(int pageIndex, int pageSize)
    {
        var request = new GetCatalogCollectionRequest
        {
            PageSize = pageSize,
            PageIndex = pageIndex
        };

        await Should.ThrowAsync<ValidationException>(async () =>
            await this._fixture.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, result => { }));
    }

    [Theory(DisplayName = "Should Validate Search Request Correctly")]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task Should_Validate_Search_Request_Correctly(int pageIndex, int pageSize)
    {
        var request = new GetCatalogCollectionRequest
        {
            PageSize = pageSize,
            PageIndex = pageIndex
        };

        await this._fixture.ExecuteValidationTest(request, result =>
        {
            if (pageIndex < 0 || pageIndex == int.MaxValue)
            {
                result.ShouldHaveValidationErrorFor(x => x.PageIndex);
            }

            if (pageSize < 0 || pageSize == int.MaxValue)
            {
                result.ShouldHaveValidationErrorFor(x => x.PageSize);
            }
        });
    }

    [Fact(DisplayName = "Should GetCatalogCollection With SearchTerm Correctly")]
    public async Task Should_GetCatalogCollection_With_SearchTerm_Correctly()
    {
        var randomIndex = GenFu.GenFu.Random.Next(0, this._fixture.Catalogs.Count);
        var catalogAtRandomIndex = this._fixture.Catalogs[randomIndex];
        var searchTerm = catalogAtRandomIndex.DisplayName;

        var request = new GetCatalogCollectionRequest
        {
            SearchTerm = searchTerm
        };

        await this._fixture.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalCatalogs.ShouldBe(1);
            result.CatalogItems.ShouldNotBeEmpty();
        });
    }

    [Fact(DisplayName = "Return empty if not found any Catalog")]
    public async Task Return_Empty_If_NotFound_Any_Catalog()
    {
        var request = A.New<GetCatalogCollectionRequest>();

        await this._fixture.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalCatalogs.ShouldBe(0);
            result.CatalogItems.ShouldBeEmpty();
        });
    }
}
