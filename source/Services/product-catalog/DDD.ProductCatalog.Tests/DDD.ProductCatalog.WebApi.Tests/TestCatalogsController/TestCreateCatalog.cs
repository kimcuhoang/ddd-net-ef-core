using AutoFixture.Xunit2;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.WebApi.Tests.Helpers;
using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit.Abstractions;

namespace DDD.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestCreateCatalog : TestBase<TestCatalogsControllerFixture>
{
    public TestCreateCatalog(ITestOutputHelper testOutput, TestCatalogsControllerFixture fixture)
        : base(testOutput, fixture)
    {
    }

    private string ApiUrl => $"{this._fixture.BaseUrl}/create";

    private Category Category => this._fixture.Category;



    [Theory(DisplayName = "Create Catalog Successfully")]
    [AutoData]
    public async Task Create_Catalog_Successfully(string catalogName)
    {
        var command = new CreateCatalogCommand
        {
            CatalogName = catalogName
        };

        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {


            var content = command.ToStringContent(jsonSerializerOptions);
            var response = await client.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();

            var model = this._fixture.Parse<CreateCatalogResult>(result);

            model.ShouldNotBeNull();
            model.CatalogId.ShouldNotBeNull();
            model.CatalogId.ShouldNotBe(CatalogId.Empty);
        });
    }

    [Theory(DisplayName = "Create Catalog Within CatalogCategory Successfully")]
    [AutoData]
    public async Task Create_Catalog_Within_CatalogCategory_Successfully(string catalogName)
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
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();

            var model = this._fixture.Parse<CreateCatalogResult>(result);

            model.ShouldNotBeNull();
            model.CatalogId.ShouldNotBeNull();
            model.CatalogId.ShouldNotBe(CatalogId.Empty);

            await this._fixture.ExecuteServiceAsync(async serviceProvider =>
            {
                var repository = serviceProvider.GetRequiredService<IRepository<Catalog, CatalogId>>();

                var catalogs = repository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories.Where(_ => _.CategoryId == this.Category.Id)
                    where c.Id == model.CatalogId
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

                var queryResult = await query.FirstOrDefaultAsync();

                queryResult.ShouldNotBeNull();
                queryResult.Catalog.ShouldNotBeNull();
                queryResult.CatalogCategory.ShouldNotBeNull();
            });
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
            var errorResponse = this._fixture.Parse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result);

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
