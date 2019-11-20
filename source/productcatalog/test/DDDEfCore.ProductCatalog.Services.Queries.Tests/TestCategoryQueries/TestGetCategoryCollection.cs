using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection;
using GenFu;
using MediatR;
using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using RequestHandler = DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection.RequestHandler;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCategoryCollection : IClassFixture<TestGetCategoryFixture>
    {
        private readonly TestGetCategoryFixture _testFixture;
        private readonly CancellationToken _cancellationToken;

        public TestGetCategoryCollection(TestGetCategoryFixture testFixture)
        {
            this._testFixture = testFixture;
            this._cancellationToken = new CancellationToken(false);
        }

        private async Task ExecuteTestWithAssert(GetCategoryCollectionRequest request, Action<GetCategoryCollectionResult> assertFor)
        {
            await this._testFixture.ExecuteScopeAsync(async dbConnection =>
            {
                IRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult> requestHandler =
                    new RequestHandler(dbConnection);

                var result = await requestHandler.Handle(request, this._cancellationToken);

                assertFor(result);
            });
        }

        [Theory(DisplayName = "Should GetCategoryCollection With Paging Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Should_GetCategoryCollection_WithPaging_Correctly(int pageIndex, int pageSize)
        {
            var categories = this._testFixture.Categories.ToList();
            var request = new GetCategoryCollectionRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await this.ExecuteTestWithAssert(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(categories.Count);
                foreach (var categoryResult in result.Categories)
                {
                    var category = categories.SingleOrDefault(x => x.CategoryId == categoryResult.Id);
                    category.ShouldNotBeNull(() => $"Assert{categoryResult.Id} in {string.Join(",", categories.Select(x => x.CategoryId.Id.ToString()).ToArray())}");
                    categoryResult.DisplayName.ShouldBe(category.DisplayName);
                }
            });
        }

        [Fact(DisplayName = "Should GetCategoryCollection With SearchTerm Correctly")]
        public async Task Should_GetCategoryCollection_With_SearchTerm_Correctly()
        {
            var randomIndex = GenFu.GenFu.Random.Next(0, this._testFixture.Categories.Count);
            var categoryAtRandomIndex = this._testFixture.Categories[randomIndex];
            var searchTerm = categoryAtRandomIndex.DisplayName;

            var request = new GetCategoryCollectionRequest
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SearchTerm = searchTerm
            };

            await this.ExecuteTestWithAssert(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(1);
                var category =
                    result.Categories.SingleOrDefault(x => x.Id == categoryAtRandomIndex.CategoryId);
                category.ShouldNotBeNull();
                category.DisplayName.ShouldBe(categoryAtRandomIndex.DisplayName);
            });
        }

        [Fact(DisplayName = "Return empty if not found any Category")]
        public async Task Return_Empty_If_NotFound_Any_Category()
        {
            var request = A.New<GetCategoryCollectionRequest>();

            await this.ExecuteTestWithAssert(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(0);
                result.Categories.ShouldBeEmpty();
            });
        }
    }
}
