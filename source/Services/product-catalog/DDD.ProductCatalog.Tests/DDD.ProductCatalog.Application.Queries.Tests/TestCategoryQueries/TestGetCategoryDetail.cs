using DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryDetail;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCategoryQueries;

public class TestGetCategoryDetail : TestCategoryQueriesBase
{
    public TestGetCategoryDetail(TestQueriesFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    [Fact(DisplayName = "Should get CategoryDetail within assigned Catalogs Correctly")]
    public async Task Should_Get_Category_Within_AssignedCatalogs_Correctly()
    {
        var category = this.Category;
        var request = new GetCategoryDetailRequest
        {
            CategoryId = this.Category.Id
        };

        await this.ExecuteTestRequestHandler<GetCategoryDetailRequest, GetCategoryDetailResult>(request, result =>
        {
            var predefinedCatalog = this.Catalog;

            result.ShouldNotBeNull();
            result.CategoryDetail.Id.ShouldBe(category.Id);
            result.CategoryDetail.DisplayName.ShouldBe(category.DisplayName);
            result.TotalCatalogs.ShouldBe(1);
            result.AssignedToCatalogs.ToList().ForEach(catalog =>
            {
                catalog.Id.ShouldBe(predefinedCatalog.Id);
                catalog.DisplayName.ShouldBe(predefinedCatalog.DisplayName);
            });
        });
    }

    [Fact(DisplayName = "Not found CategoryDetail Should Not Throw Exception")]
    public async Task NotFound_Category_Should_Not_ThrowException()
    {
        var request = new GetCategoryDetailRequest
        {
            CategoryId = CategoryId.New
        };

        await this.ExecuteTestRequestHandler<GetCategoryDetailRequest, GetCategoryDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CategoryDetail.Id.ShouldBeNull();
            result.CategoryDetail.DisplayName.ShouldBeNullOrWhiteSpace();
            result.TotalCatalogs.ShouldBe(0);
        });
    }

    [Fact(DisplayName = "Invalid CategoryId Should Be Fail For Validation")]
    public async Task Invalid_CategoryId_ShouldBe_Fail_For_Validation()
    {
        var request = new GetCategoryDetailRequest
        {
            CategoryId = CategoryId.Empty
        };
        await this.ExecuteValidationTest(request, result => { result.ShouldHaveValidationErrorFor(x => x.CategoryId); });
    }
}
