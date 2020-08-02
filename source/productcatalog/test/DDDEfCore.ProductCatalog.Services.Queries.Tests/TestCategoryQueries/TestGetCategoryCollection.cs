using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection;
using GenFu;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
        [InlineData(1, 1)]
        [InlineData(1, 2)]
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
                categoryResult.Id.ShouldBe(category.Id);
                categoryResult.DisplayName.ShouldBe(category.DisplayName);
            });
        }

        [Fact(DisplayName = "Should GetCategoryCollection With SearchTerm Correctly")]
        public async Task Should_GetCategoryCollection_With_SearchTerm_Correctly()
        {
            var category = this._testFixture.Category;
            var request = new GetCategoryCollectionRequest
            {
                SearchTerm = category.DisplayName
            };

            await this._testFixture.ExecuteTestRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult>(request, (result) =>
            {
                result.ShouldNotBeNull();
                result.TotalCategories.ShouldBe(1);
                result.Categories.ShouldHaveSingleItem();
                var categoryResult = result.Categories.FirstOrDefault();
                categoryResult.Id.ShouldBe(category.Id);
                categoryResult.DisplayName.ShouldBe(category.DisplayName);
            });
        }

        [Theory(DisplayName = "Invalid Search Request Should Throw ValidationException")]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Invalid_Search_Request_Should_Throw_ValidationException(int pageIndex, int pageSize)
        {
            var request = new GetCategoryCollectionRequest
            {
                PageSize = pageSize,
                PageIndex = pageIndex
            };

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._testFixture.ExecuteTestRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult>(request, result => { }));
        }

        [Theory(DisplayName = "Should Validate Search Request Correctly")]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Should_Validate_Search_Request_Correctly(int pageIndex, int pageSize)
        {
            var request = new GetCategoryCollectionRequest
            {
                PageSize = pageSize,
                PageIndex = pageIndex
            };

            await this._testFixture.ExecuteValidationTest(request, result =>
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
