using DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;
using DDD.ProductCatalog.Core.Products;
using DDD.ProductCatalog.WebApi.Infrastructures.JsonConverters;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Xunit.Abstractions;

namespace DDD.ProductCatalog.WebApi.Tests;
public class TestJsonSerialization
{
    private readonly ITestOutputHelper _output;

    public TestJsonSerialization(ITestOutputHelper output)
    {
        this._output = output;
    }

    [Fact]
    public void Can_Serialize_StronglyTypedId()
    {
        GetProductCollectionResult result = new()
        {
            TotalProducts = 10,
            Products = new List<GetProductCollectionResult.ProductCollectionItem>
            {
                new GetProductCollectionResult.ProductCollectionItem
                {
                    Id = ProductId.New,
                    DisplayName = "This is a test product"
                }
            }
        };

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        jsonSerializerOptions.Converters.Add(new IdentityJsonConverterFactory());

        var resultAsJsonString01 = JsonSerializer.Serialize(result, jsonSerializerOptions);

        this._output.WriteLine(resultAsJsonString01);

        var deserializedFromJsonString = JsonSerializer.Deserialize<GetProductCollectionResult>(resultAsJsonString01, jsonSerializerOptions);

        deserializedFromJsonString.ShouldNotBeNull();

        var resultAsJsonString02 = JsonSerializer.Serialize(deserializedFromJsonString, jsonSerializerOptions);

        this._output.WriteLine(resultAsJsonString02);

        resultAsJsonString02.ShouldBe(resultAsJsonString01);
    }
}
