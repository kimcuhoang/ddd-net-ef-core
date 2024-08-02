using DNK.DDD.Core.Models;
using System.Reflection;

namespace DDD.ProductCatalog.Core;

public static class StronglyTypedIdTypeDescriptor
{
    public static void AddStronglyTypedIdConverter(Action<Type> additionalAction)
    {
        Assembly.GetExecutingAssembly()
            .ExportedTypes
            .Where(x => !x.IsGenericTypeDefinition && !x.IsAbstract && x.BaseType == typeof(IdentityBase))
            .ToList().ForEach(idType =>
            {
                additionalAction?.Invoke(idType);
            });
    }
}
