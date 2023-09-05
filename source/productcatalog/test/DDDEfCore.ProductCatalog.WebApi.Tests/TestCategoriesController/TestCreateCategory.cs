using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestCreateCategory : TestBase<TestCategoryControllerFixture>
{
    public TestCreateCategory(ITestOutputHelper testOutput, TestCategoryControllerFixture fixture) 
        : base(testOutput, fixture)
    {
    }

    private string ApiUrl => $"{this._fixture.BaseUrl}/create";

    

    [Theory(DisplayName = "Create Category Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Create_Category_Successfully_Should_Return_HttpStatusCode204(string categoryName)
    {
        await this._fixture.DoTest(async (client, jsonSerializeOptions) =>
        {
            var content = new { CategoryName = categoryName }.ToStringContent(jsonSerializeOptions);
            var response = await client.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var model = this._fixture.Parse<CreateCategoryResult>(result);

            model.ShouldNotBeNull();
            model.CategoryId.ShouldNotBeNull().ShouldNotBe(CategoryId.Empty);

            await this._fixture.ExecuteDbContextAsync(async dbContext =>
            {
                var category = await dbContext.Set<Category>().FirstOrDefaultAsync(_ => _.Id == model.CategoryId);

                category.ShouldNotBeNull();
            });
        });


    }

    [Fact(DisplayName = "Create Category With Empty Name Should Return HttpStatusCode400")]
    public async Task Create_Category_With_EmptyName_Should_Return_HttpStatusCode400()
    {
        await this._fixture.DoTest(async (client, jsonSerializeOptions) =>
        {
            var content = string.Empty.ToStringContent();
            var response = await client.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        });
    }
}
