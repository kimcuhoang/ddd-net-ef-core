using System.Text.Json;
using System.Text.Json.Serialization;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.WebApi.Infrastructures.JsonConverters;

public class IdentityJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return !typeToConvert.IsGenericType 
                && typeToConvert.BaseType != null && typeToConvert.BaseType == typeof(IdentityBase);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(IdentityJsonConverter<>).MakeGenericType(typeToConvert);

        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}
