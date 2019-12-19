using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.Helpers
{
    public static class ContentHelper
    {
        public static StringContent ToStringContent(this object obj, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (jsonSerializerOptions == null)
            {
                jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            }
                
            return new StringContent(JsonSerializer.Serialize(obj, jsonSerializerOptions), Encoding.UTF8, "application/json");
        }
    }
}
