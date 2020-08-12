using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.WebApi.Infrastructures.JsonConverters
{
    /// <summary>
    /// https://weblogs.thinktecture.com/pawel/2019/10/aspnet-core-3-0-custom-jsonconverter-for-the-new-system_text_json.html
    /// </summary>
    public class IdentityJsonConverter<TIdentity> : JsonConverter<TIdentity> where TIdentity : IdentityBase
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(TIdentity);
        }

        public override TIdentity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //return IdentityFactory.Create<TIdentity>(reader.GetGuid());
            return (TIdentity)Activator.CreateInstance(type: typeof(TIdentity),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { reader.GetGuid() },
                culture: null);
        }

        public override void Write(Utf8JsonWriter writer, TIdentity value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }
}
