using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDDEfCore.ProductCatalog.WebApi.Tests.Helpers;
using Shouldly;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController
{
    [Collection(nameof(SharedFixture))]
    public class TestUpdateCatalog : IClassFixture<TestCatalogsControllerFixture>
    {
        private readonly TestCatalogsControllerFixture _testCatalogsControllerFixture;

        private Catalog Catalog => this._testCatalogsControllerFixture.Catalog;

        private string ApiUrl => $"{this._testCatalogsControllerFixture.BaseUrl}/{(Guid) this.Catalog.Id}";

        public TestUpdateCatalog(TestCatalogsControllerFixture testCatalogsControllerFixture)
            => this._testCatalogsControllerFixture = testCatalogsControllerFixture;

        [Theory(DisplayName = "Update Catalog Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Update_Catalog_Successfully_Should_Return_HttpStatusCode204(string catalogName)
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var content = catalogName.ToStringContent();
                var response = await client.PutAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Update Catalog With Empty Name Should Return HttpStatusCode400")]
        public async Task Update_Catalog_With_Empty_Name_Should_Return_HttpStatusCode400()
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions) =>
            {
                var content = string.Empty.ToStringContent();
                var response = await client.PutAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

                var result = await response.Content.ReadAsStringAsync();
                var errorResponse =
                    JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result,
                        jsonSerializationOptions);

                errorResponse.ShouldNotBeNull();
                errorResponse.Status.ShouldBe((int)HttpStatusCode.BadRequest);
                errorResponse.ErrorMessages.ShouldNotBeEmpty();
            });
        }
    }
}
