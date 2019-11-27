using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController
{
    [Collection(nameof(SharedFixture))]
    public class TestCreateCatalog : IClassFixture<TestCatalogsControllerFixture>
    {
        private readonly TestCatalogsControllerFixture _testCatalogsControllerFixture;

        private string ApiUrl => $"{this._testCatalogsControllerFixture.BaseUrl}/create";

        private Category Category => this._testCatalogsControllerFixture.Category;

        public TestCreateCatalog(TestCatalogsControllerFixture testCatalogsControllerFixture)
            => this._testCatalogsControllerFixture = testCatalogsControllerFixture;

        [Theory(DisplayName = "Create Catalog Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Create_Catalog_Successfully_Should_Return_HttpStatusCode204(string catalogName)
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
            {
                var command = new CreateCatalogCommand
                {
                    CatalogName = catalogName
                };

                var jsonData = JsonSerializer.Serialize(command);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Theory(DisplayName = "Create Catalog Within CatalogCategory Successfully Should Return HttpStatusCode204")]
        [AutoData]
        public async Task Create_Catalog_Within_CatalogCategory_Successfully_Should_Return_HttpStatusCode204(string catalogName)
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
            {
                var command = new CreateCatalogCommand
                {
                    CatalogName = catalogName
                };
                command.AddCategory(this.Category.CategoryId, this.Category.DisplayName);

                var jsonData = JsonSerializer.Serialize(command, jsonSerializationOptions);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
                response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            });
        }

        [Fact(DisplayName = "Empty Catalog Name Should Return HttpStatusCode400")]
        public async Task Empty_CatalogName_Should_Return_HttpStatusCode400()
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
            {
                var command = new CreateCatalogCommand();

                var jsonData = JsonSerializer.Serialize(command);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
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

        [Theory(DisplayName = "Invalid CategoryId Should Return HttpStatusCode400")]
        [MemberData(nameof(InvalidCategoryIds))]
        public async Task Invalid_CategoryId_Should_Return_HttpStatusCode400(Guid categoryId)
        {
            await this._testCatalogsControllerFixture.DoTest(async (client, jsonSerializationOptions, services) =>
            {
                var command = new CreateCatalogCommand
                {
                    CatalogName = "CatalogName"
                };
                command.AddCategory(IdentityFactory.Create<CategoryId>(categoryId), this.Category.DisplayName);

                var jsonData = JsonSerializer.Serialize(command,jsonSerializationOptions);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(this.ApiUrl, content);
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

        public static IEnumerable<object[]> InvalidCategoryIds => new List<object[]>
        {
            new object[] { Guid.Empty },
            new object[] { Guid.NewGuid()}
        };
    }
}
