using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Application.Commands.CatalogCommands.UpdateCatalog;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestUpdateCatalog : TestCatalogsControllerBase
{
    public TestUpdateCatalog(WebApiTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : base(testCollectionFixture, output)
    {
    }

    private string ApiUrl => $"{this.BaseUrl}/{(Guid)this.Catalog.Id}";



    [Theory(DisplayName = "Update Catalog Successfully")]
    [AutoData]
    public async Task Update_Catalog_Successfully_Should_Return(string catalogName)
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(catalogName);
            
            var response = await httpClient.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var model = await this.ParseResponse<UpdateCatalogResult>(response);

            model.ShouldNotBeNull();
            model.CatalogId.ShouldNotBeNull();
            model.Success.ShouldBeTrue();
        });

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var catalog = await dbContext.Set<Catalog>().FirstOrDefaultAsync(_ => _.Id == this.Catalog.Id);

            catalog.ShouldNotBeNull();
            catalog.DisplayName.ShouldBe(catalogName);
        });
    }

    [Fact(DisplayName = "Update Catalog With Empty Name Should Return HttpStatusCode400")]
    public async Task Update_Catalog_With_Empty_Name_Should_Return_HttpStatusCode400()
    {
        await this.ExecuteHttpClientAsync(async httpClient =>
        {
            var content = this.ConvertRequestToStringContent(string.Empty);

            var response = await httpClient.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var errorResponse = await this.ParseResponse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(response);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }
}
