using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection;
using GenFu;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCategoryCollection : IClassFixture<TestGetCategoryFixture>
    {
        private readonly TestGetCategoryFixture _testFixture;

        public TestGetCategoryCollection(TestGetCategoryFixture testFixture)
        {
            this._testFixture = testFixture;
        }

        [Theory(DisplayName = "Should GetCategoryCollection With Paging Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        public async Task Should_GetCategoryCollection_WithPaging_Correctly(int pageIndex, int pageSize)
        {
            var request = new GetCategoryCollectionRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await this._testFixture.ExecuteTestRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(1);
                result.Categories.ShouldHaveSingleItem();

                var category = this._testFixture.Category;
                var categoryResult = result.Categories.FirstOrDefault();
                categoryResult.Id.ShouldBe(category.CategoryId);
                categoryResult.DisplayName.ShouldBe(category.DisplayName);
            });
        }

        [Fact(DisplayName = "Should GetCategoryCollection With SearchTerm Correctly")]
        public async Task Should_GetCategoryCollection_With_SearchTerm_Correctly()
        {
            var category = this._testFixture.Category;
            var request = new GetCategoryCollectionRequest
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SearchTerm = category.DisplayName
            };

            await this._testFixture.ExecuteTestRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(1);
                result.Categories.ShouldHaveSingleItem();
                var categoryResult = result.Categories.FirstOrDefault();
                categoryResult.Id.ShouldBe(category.CategoryId);
                categoryResult.DisplayName.ShouldBe(category.DisplayName);
            });
        }

        [Fact(DisplayName = "Return empty if not found any CategoryDetail")]
        public async Task Return_Empty_If_NotFound_Any_Category()
        {
            var request = A.New<GetCategoryCollectionRequest>();

            await this._testFixture.ExecuteTestRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(0);
                result.Categories.ShouldBeEmpty();
            });
        }
    }
}
