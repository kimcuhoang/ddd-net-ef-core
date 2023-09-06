﻿using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using Shouldly;
using System.Net;
using System.Text.Json;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController;

public class TestSearchCategories : TestBase<TestCategoryControllerFixture>
{
    public TestSearchCategories(ITestOutputHelper testOutput, TestCategoryControllerFixture fixture) : base(testOutput, fixture)
    {
    }

    private Category Category => this._fixture.Category;

    private IEnumerable<Category> Categories => new List<Category> {this.Category};

    private string ApiUrl => $"{this._fixture.BaseUrl}/search";

    [Theory(DisplayName = "Can Search Categories With Paging Correctly And Return HttpStatusCode200")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(1, null)]
    [InlineData(null, 1)]
    [InlineData(null, null)]
    public async Task Can_Search_Categories_With_Paging_Correctly_And_Return_HttpStatusCode200(int? pageIndex, int? pageSize)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("searchTerm", this.Category.DisplayName);

            if (pageIndex.HasValue)
            {
                parameters.Add(nameof(pageIndex), $"{pageIndex.Value}");
            }

            if (pageSize.HasValue)
            {
                parameters.Add(nameof(pageSize), $"{pageSize.Value}");
            }

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

            var response = await client.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var categoryResult =
                JsonSerializer.Deserialize<GetCategoryCollectionResult>(result, jsonSerializerOptions);

            categoryResult.ShouldNotBeNull();
            categoryResult.TotalCategories.ShouldBe(this.Categories.Count());
            categoryResult.Categories.ToList().ForEach(category =>
            {
                var predefinedCategory = this.Categories.SingleOrDefault(x => x.Id == category.Id && x.DisplayName == category.DisplayName);
                predefinedCategory.ShouldNotBeNull();
            });
        });
    }

    [Theory(DisplayName = "Search not found still return HttpStatusCode200")]
    [AutoData]
    public async Task Search_Not_Found_Still_Return_HttpStatusCode200(string randomSearchTerm)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("searchTerm", randomSearchTerm);

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";

            var response = await client.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            var categoryResult =
                JsonSerializer.Deserialize<GetCategoryCollectionResult>(result, jsonSerializerOptions);

            categoryResult.ShouldNotBeNull();
            categoryResult.TotalCategories.ShouldBe(0);
            categoryResult.Categories.ShouldBeEmpty();
        });
    }

    [Theory(DisplayName = "Invalid Search Request Should Return HttpStatusCode400")]
    [InlineData(1, 0)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    public async Task Invalid_Search_Request_Should_Return_HttpStatusCode400(int pageIndex, int pageSize)
    {
        await this._fixture.DoTest(async (client, jsonSerializerOptions) =>
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add(nameof(pageIndex), $"{pageIndex}");
            parameters.Add(nameof(pageSize), $"{pageSize}");

            var searchUrl = $"{this.ApiUrl}?{parameters.ToString()}";
            var response = await client.GetAsync(searchUrl);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadAsStringAsync();
            var searchResult =
                JsonSerializer.Deserialize<GlobalExceptionHandlerMiddleware.ExceptionResponse>(result, jsonSerializerOptions);
            searchResult.Status.ShouldBe((int)HttpStatusCode.BadRequest);
            searchResult.ErrorMessages.Count.ShouldBeGreaterThan(0);
        });
    }
}
