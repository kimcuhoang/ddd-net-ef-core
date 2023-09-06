using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.WebApi.Tests;

[Collection(nameof(WebApiTestCollection))]
public abstract class TestBase<TTestFixture> : IClassFixture<TTestFixture>
    where TTestFixture : DefaultTestFixture
{
    protected readonly ITestOutputHelper _testOutput;
    protected readonly TTestFixture _fixture;

    protected TestBase(ITestOutputHelper testOutput, TTestFixture fixture)
    {
        this._testOutput = testOutput;
        this._fixture = fixture;
    }

    protected StringContent ConvertToStringContent(object input)
    {
        var jsonSerializerOptions = this._fixture.JsonSerializerOptions;

        var inputAsJson = JsonSerializer.Serialize(input, jsonSerializerOptions);

        var stringContent = new StringContent(inputAsJson, Encoding.UTF8, "application/json");

        return stringContent;
    }
}
