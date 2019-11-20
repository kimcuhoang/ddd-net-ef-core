using System;
using DDDEfCore.Core.Common.Models;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DDDEfCore.ProductCatalog.Core.DomainModels
{
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
}
