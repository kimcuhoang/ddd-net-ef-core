using DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogCollections;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCatalogQueries;

public class TestGetCatalogCollection : TestCatalogQueriesBase
{
    public TestGetCatalogCollection(TestQueriesCollectionFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    [Theory(DisplayName = "Should GetCatalogCollection With Paging Correctly")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    public async Task Should_GetCatalogCollection_WithPaging_Correctly(int pageIndex, int pageSize)
    {
        var catalogs = this.Catalogs.ToList();
        var request = new GetCatalogCollectionRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        await this.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalCatalogs.ShouldBeGreaterThanOrEqualTo(1);
            result.CatalogItems.ShouldNotBeEmpty();
        });
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

        await this.ExecuteValidationTest(request, result =>
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
        var randomIndex = GenFu.GenFu.Random.Next(0, this.Catalogs.Count);
        var catalogAtRandomIndex = this.Catalogs[randomIndex];
        var searchTerm = catalogAtRandomIndex.DisplayName;

        var request = new GetCatalogCollectionRequest
        {
            SearchTerm = searchTerm
        };

        await this.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalCatalogs.ShouldBe(1);
            result.CatalogItems.ShouldNotBeEmpty();
        });
    }

    [Fact(DisplayName = "Return empty if not found any Catalog")]
    public async Task Return_Empty_If_NotFound_Any_Catalog()
    {
        var request = GenFu.GenFu.New<GetCatalogCollectionRequest>();

        await this.ExecuteTestRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>(request, (result) =>
        {
            result.ShouldNotBeNull();
            result.TotalCatalogs.ShouldBe(0);
            result.CatalogItems.ShouldBeEmpty();
        });
    }
}
