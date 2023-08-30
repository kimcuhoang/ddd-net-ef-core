using System.ComponentModel;
using System.Reflection;
using DDDEfCore.ProductCatalog.Core.DomainModels;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore;
using DDDEfCore.ProductCatalog.Services.Commands.Infrastructure.PipelineBehaviors;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Commands.Infrastructure;

public static class ApplicationServiceCommandsRegistration
{
    public static IServiceCollection AddApplicationCommands(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddEfCoreSqlServerDb(configuration);
        services.AddMediatR(mediatR =>
        {
            mediatR
                .RegisterServicesFromAssembly(assembly)
                .AddOpenBehavior(typeof(EndRequestPipelineBehavior<,>));
        });

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssembly(assembly);

        StronglyTypedIdTypeDescriptor.AddStronglyTypedIdConverter((idType) =>
        {
            var typeOfIdentity = typeof(StronglyTypedIdConverter<>).MakeGenericType(idType);
            TypeDescriptor.AddAttributes(idType, new TypeConverterAttribute(typeOfIdentity));
        });

        return services;
    }
}
