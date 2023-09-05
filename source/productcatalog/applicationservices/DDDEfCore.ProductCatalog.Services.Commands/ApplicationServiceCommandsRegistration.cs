using System.ComponentModel;
using DDDEfCore.ProductCatalog.Core.DomainModels;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Commands;

public static class ApplicationServiceCommandsRegistration
{
    public static IServiceCollection AddApplicationCommands(this IServiceCollection services)
    {
        StronglyTypedIdTypeDescriptor.AddStronglyTypedIdConverter((idType) =>
        {
            var typeOfIdentity = typeof(StronglyTypedIdConverter<>).MakeGenericType(idType);
            TypeDescriptor.AddAttributes(idType, new TypeConverterAttribute(typeOfIdentity));
        });

        return services;
    }
}
