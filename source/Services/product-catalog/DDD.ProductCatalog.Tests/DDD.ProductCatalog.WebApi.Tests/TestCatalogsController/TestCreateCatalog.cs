using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalog;
using Microsoft.Extensions.DependencyInjection;
using DNK.DDD.Core;
using Microsoft.EntityFrameworkCore;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDD.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestCreateCatalog(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : TestCatalogsControllerBase(testCollectionFixture, output)
{
    private string ApiUrl => $"{this.BaseUrl}/create";

    [Theory(DisplayName = "Create Catalog Successfully")]
    [AutoData]
    public async Task Create_Catalog_Successfully(string catalogName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateCatalogCommand
            {
                CatalogName = catalogName
            };

            var content = this.ConvertRequestToStringContent(command);

            var response = await httpClient.PostAsync(this.ApiUrl, content);

            var model = await this.ParseResponse<CreateCatalogResult>(response);

            model.ShouldNotBeNull();
            model.CatalogId.ShouldNotBeNull();
            model.CatalogId.ShouldNotBe(CatalogId.Empty);
        });
    }

    [Theory(DisplayName = "Create Catalog Within CatalogCategory Successfully")]
    [AutoData]
    public async Task Create_Catalog_Within_CatalogCategory_Successfully(string catalogName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateCatalogCommand
            {
                CatalogName = catalogName
            };
            command.AddCategory(this.Category.Id, this.Category.DisplayName);

            var content = this.ConvertRequestToStringContent(command);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            var model = await this.ParseResponse<CreateCatalogResult>(response);

            model.ShouldNotBeNull();
            model.CatalogId.ShouldNotBeNull();
            model.CatalogId.ShouldNotBe(CatalogId.Empty);

            await this.ExecuteServiceAsync(async serviceProvider =>
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
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateCatalogCommand();

            var content = this.ConvertRequestToStringContent(command);

            var response = await httpClient.PostAsync(this.ApiUrl, content);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Theory(DisplayName = "Invalid CategoryId Should Return HttpStatusCode400")]
    [MemberData(nameof(InvalidCategoryIds))]
    public async Task Invalid_CategoryId_Should_Return_HttpStatusCode400(Guid categoryId)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var command = new CreateCatalogCommand
            {
                CatalogName = "CatalogName"
            };
            command.AddCategory(CategoryId.Of(categoryId), this.Category.DisplayName);

            var content = this.ConvertRequestToStringContent(command);
            var response = await httpClient.PostAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var errorResponse = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);
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
