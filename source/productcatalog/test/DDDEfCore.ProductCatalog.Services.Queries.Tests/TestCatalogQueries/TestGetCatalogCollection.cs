using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections;
using MediatR;
using Shouldly;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GenFu;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogCollection : IClassFixture<TestGetCatalogFixture>
    {
        private readonly TestGetCatalogFixture _testFixture;
        private readonly CancellationToken _cancellationToken;

        public TestGetCatalogCollection(TestGetCatalogFixture testFixture)
        {
            this._testFixture = testFixture;
            this._cancellationToken = new CancellationToken(false);
        }

        private async Task ExecuteTestWithAssert(GetCatalogCollectionRequest request, Action<GetCatalogCollectionResult> assertFor)
        {
            await this._testFixture.ExecuteScopeAsync(async dbConnection =>
            {
                IRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult> requestHandler =
                    new RequestHandler(dbConnection);

                var result = await requestHandler.Handle(request, this._cancellationToken);

                assertFor(result);
            });
        }

        [Theory(DisplayName = "Should GetCatalogCollection With Paging Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Should_GetCatalogCollection_WithPaging_Correctly(int pageIndex, int pageSize)
        {
            var catalogs = this._testFixture.Catalogs.ToList();
            var request = new GetCatalogCollectionRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await this.ExecuteTestWithAssert(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCatalogs.ShouldBe(catalogs.Count);
                foreach (var catalogItem in result.CatalogItems)
                {
                    var catalogId = new CatalogId(catalogItem.CatalogId);
                    var catalog = catalogs.SingleOrDefault(x => x.CatalogId == catalogId);
                    catalog.ShouldNotBeNull(() => $"Assert{catalogId} in {string.Join(",", catalogs.Select(x => x.CatalogId.Id.ToString()).ToArray())}");
                    catalogItem.DisplayName.ShouldBe(catalog.DisplayName);
                    catalogItem.TotalCategories.ShouldBe(catalog.Categories.Count());
                }
            });
        }

        [Fact(DisplayName = "Should GetCatalogCollection With SearchTerm Correctly")]
        public async Task Should_GetCatalogCollection_With_SearchTerm_Correctly()
        {
            var randomIndex = GenFu.GenFu.Random.Next(0, this._testFixture.Catalogs.Count);
            var catalogAtRandomIndex = this._testFixture.Catalogs[randomIndex];
            var searchTerm = catalogAtRandomIndex.DisplayName;

            var request = new GetCatalogCollectionRequest
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SearchTerm = searchTerm
            };

            await this.ExecuteTestWithAssert(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCatalogs.ShouldBe(1);
                var catalog =
                    result.CatalogItems.SingleOrDefault(x => x.CatalogId == catalogAtRandomIndex.CatalogId.Id);
                catalog.ShouldNotBeNull();
                catalog.DisplayName.ShouldBe(catalogAtRandomIndex.DisplayName);
                catalog.TotalCategories.ShouldBe(catalogAtRandomIndex.Categories.Count());
            });
        }

        [Fact(DisplayName = "Return empty if not found any Catalog")]
        public async Task Return_Empty_If_NotFound_Any_Catalog()
        {
            var request = A.New<GetCatalogCollectionRequest>();

            await this.ExecuteTestWithAssert(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCatalogs.ShouldBe(0);
                result.CatalogItems.ShouldBeEmpty();
            });
        }
    }
}
