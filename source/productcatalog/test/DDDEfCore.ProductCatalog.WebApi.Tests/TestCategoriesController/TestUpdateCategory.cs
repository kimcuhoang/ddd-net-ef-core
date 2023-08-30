using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestUpdateCategory : TestBase<TestCategoryControllerFixture>
{
    public TestUpdateCategory(ITestOutputHelper testOutput, TestCategoryControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private Category Category => this._fixture.Category;
    public string ApiUrl => $"{this._fixture.BaseUrl}/{(Guid)this.Category.Id}";

    [Theory(DisplayName = "Update Category Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Update_Category_Successfully_Should_Return_HttpStatusCode204(string categoryName)
    {
        await this._fixture.DoTest(async (client, jsonSerializeOptions) =>
        {
            var content = categoryName.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        });
    }

    [Fact(DisplayName = "Update Category with Empty Name Should Return HttpStatusCode400")]
    public async Task Update_Category_With_Empty_Name_Should_Return_HttpStatusCode400()
    {
        await this._fixture.DoTest(async (client, jsonSerializeOptions) =>
        {
            var content = string.Empty.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        });
    }
}
