using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections;
using MediatR;
using Shouldly;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogCollection : IClassFixture<TestGetCatalogCollectionFixture>
    {
        private readonly TestGetCatalogCollectionFixture _testFixture;

        public TestGetCatalogCollection(TestGetCatalogCollectionFixture testFixture)
        {
            this._testFixture = testFixture;
        }


        [Theory(DisplayName = "Should GetCatalogCollection With Paging Correctly")]
        [InlineData(1, 1000)]
        [InlineData(10, 1000)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public async Task Should_GetCatalogCollection_WithPaging_Correctly(int pageIndex, int pageSize)
        {
            var request = new GetCatalogCollectionRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await this._testFixture.ExecuteScopeAsync(async dbConnection =>
            {
                IRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult> requestHandler =
                    new RequestHandler(dbConnection);

                var result = await requestHandler.Handle(request, CancellationToken.None);

                result.ShouldNotBeNull();
                result.TotalCatalogs.ShouldBe(this._testFixture.Catalogs.Count);
                foreach (var catalogItem in result.CatalogItems)
                {
                    var catalog =
                        this._testFixture.Catalogs.SingleOrDefault(x => x.CatalogId == new CatalogId(catalogItem.CatalogId));
                    catalog.ShouldNotBeNull();
                    catalogItem.DisplayName.ShouldBe(catalog.DisplayName);
                    catalogItem.TotalCategories.ShouldBe(catalog.Categories.Count());
                }
            });
        }

        [Fact(DisplayName = "Should_GetCatalogCollection_With_SearchTerm_Correctly")]
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

            await this._testFixture.ExecuteScopeAsync(async dbConnection =>
            {
                IRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult> requestHandler =
                    new RequestHandler(dbConnection);

                var result = await requestHandler.Handle(request, CancellationToken.None);

                result.ShouldNotBeNull();
                result.TotalCatalogs.ShouldBe(1);
                var catalog =
                    result.CatalogItems.SingleOrDefault(x => x.CatalogId == catalogAtRandomIndex.CatalogId.Id);
                catalog.ShouldNotBeNull();
                catalog.DisplayName.ShouldBe(catalogAtRandomIndex.DisplayName);
                catalog.TotalCategories.ShouldBe(catalogAtRandomIndex.Categories.Count());
            });
        }
    }
}
