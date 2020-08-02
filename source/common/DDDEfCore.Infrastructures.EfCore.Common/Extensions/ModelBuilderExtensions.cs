using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.Infrastructures.EfCore.Common.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Register(this ModelBuilder builder, IEnumerable<Assembly> fromAssemblies)
        {
            var types = fromAssemblies.SelectMany(x => x.DefinedTypes);

            if (types?.Any() == true)
            {
                builder.RegisterEntities(types);
                builder.RegisterCustomMappings(types);
            }
        }

        private static void RegisterEntities(this ModelBuilder builder, IEnumerable<Type> fromTypes)
        {
            var concreteTypes = fromTypes
                .Where(x => !x.GetTypeInfo().IsAbstract 
                            && !x.GetTypeInfo().IsInterface
                            && x.BaseType != null
                            && x.IsConcreteOfAggregateRoot());

            foreach (var concreteType in concreteTypes)
            {
                builder.Entity(concreteType);
            }
        }

        private static void RegisterCustomMappings(this ModelBuilder builder, IEnumerable<Type> fromTypes)
        {
            var customModelBuilderTypes = fromTypes.Where(x => x != null 
                                                                && typeof(ICustomModelBuilder).IsAssignableFrom(x)
                                                                && x != typeof(ICustomModelBuilder));

            foreach (var builderType in customModelBuilderTypes)
            {
                var customModelBuilder = (ICustomModelBuilder)Activator.CreateInstance(builderType);
                customModelBuilder.Build(builder);
            }
        }

        private static bool IsConcreteOfAggregateRoot(this Type type)
        {
            return typeof(AggregateRoot<>).IsAssignableFrom(type)
                   || (type.GetTypeInfo().BaseType.IsGenericType &&
                       typeof(AggregateRoot<>).IsAssignableFrom(type.GetTypeInfo().BaseType.GetGenericTypeDefinition()));
        }
    }
}
