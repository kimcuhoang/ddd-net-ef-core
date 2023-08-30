using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System.Net;
using System.Text.Json;
using Xunit;
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

    

    [Theory(DisplayName = "Update Catalog Successfully Should Return HttpStatusCode204")]
    [AutoData]
    public async Task Update_Catalog_Successfully_Should_Return_HttpStatusCode204(string catalogName)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var content = catalogName.ToStringContent();
            var response = await client.PutAsync(this.ApiUrl, content);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
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
            var errorResponse =
                JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                    jsonSerializerOptions);

            errorResponse.ShouldNotBeNull();
            errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            errorResponse.ErrorMessages.ShouldNotBeEmpty();
        });
    }
}
