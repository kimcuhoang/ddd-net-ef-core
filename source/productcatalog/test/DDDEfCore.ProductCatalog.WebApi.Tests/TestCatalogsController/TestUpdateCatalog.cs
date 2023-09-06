using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController;

public class TestUpdateCatalog : TestBase<TestCatalogsControllerFixture>
{
    public TestUpdateCatalog(ITestOutputHelper testOutput, TestCatalogsControllerFixture fixture) 
        : base(testOutput, fixture)
    {
    }

    private Catalog Catalog => this._fixture.Catalog;

    private string ApiUrl => $"{this._fixture.BaseUrl}/{(Guid) this.Catalog.Id}";

    

    [Theory(DisplayName = "Update Catalog Successfully")]
    [AutoData]
    public async Task Update_Catalog_Successfully_Should_Return(string catalogName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var content = catalogName.ToStringContent(jsonSerializerOptions);
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();

            var model = this._fixture.Parse<UpdateCatalogResult>(result);

            model.ShouldNotBeNull();
            model.CatalogId.ShouldNotBeNull();
            model.Success.ShouldBeTrue();
        });

        await this._fixture.ExecuteDbContextAsync(async dbContext =>
        {
            var catalog = await dbContext.Set<Catalog>().FirstOrDefaultAsync(_ => _.Id == this.Catalog.Id);

            catalog.ShouldNotBeNull();
            catalog.DisplayName.ShouldBe(catalogName);
        });
    }

    [Fact(DisplayName = "Update Catalog With Empty Name Should Return HttpStatusCode400")]
    public async Task Update_Catalog_With_Empty_Name_Should_Return_HttpStatusCode400()
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var content = string.Empty.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();
            var errorResponse = this._fixture.Parse<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }
}
