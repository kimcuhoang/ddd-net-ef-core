using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System.Net;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestCreateCatalog : TestBase<TestCatalogsControllerFixture>
{
    public TestCreateCatalog(ITestOutputHelper testOutput, TestCatalogsControllerFixture fixture) 
        : base(testOutput, fixture)
    {
    }

    private string ApiUrl => $"{this._fixture.BaseUrl}/create";

    private Category Category => this._fixture.Category;

    

    [Theory(DisplayName = "Create Catalog Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Create_Catalog_Successfully_Should_Return_HttpStatusCode204(string catalogName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var command = new CreateCatalogCommand
            {
                CatalogName = catalogName
            };

            var content = command.ToStringContent();
            var response = await client.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        });
    }

    [Theory(DisplayName = "Create Catalog Within CatalogCategory Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Create_Catalog_Within_CatalogCategory_Successfully_Should_Return_HttpStatusCode204(string catalogName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var command = new CreateCatalogCommand
            {
                CatalogName = catalogName
            };
            command.AddCategory(this.Category.Id, this.Category.DisplayName);

            var content = command.ToStringContent(jsonSerializerOptions);
            var response = await client.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        });
    }

    [Fact(DisplayName = "Empty Catalog Name Should Return HttpStatusCode400")]
    public async Task Empty_CatalogName_Should_Return_HttpStatusCode400()
    {
        await this._fixture.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateCatalogCommand();

            var content = this.ConvertToStringContent(command);

            var response = await httpClient.PostAsync(this.ApiUrl, content);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();
            var errorResponse = this._fixture.Parse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }

    [Theory(DisplayName = "Invalid CategoryId Should Return HttpStatusCode400")]
    [MemberData(nameof(InvalidCategoryIds))]
    public async Task Invalid_CategoryId_Should_Return_HttpStatusCode400(Guid categoryId)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var command = new CreateCatalogCommand
            {
                CatalogName = "CatalogName"
            };
            command.AddCategory(CategoryId.Of(categoryId), this.Category.DisplayName);

            var content = command.ToStringContent(jsonSerializerOptions);
            var response = await client.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();
            var errorResponse =
                JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                    jsonSerializerOptions);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }

    public static IEnumerable<object[]> InvalidCategoryIds => new List<object[]>
    {
        new object[] { Guid.Empty },
        new object[] { Guid.NewGuid()}
    };
}
