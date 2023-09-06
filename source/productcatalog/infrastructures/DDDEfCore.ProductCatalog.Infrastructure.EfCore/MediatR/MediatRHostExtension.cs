using DDDEfCore.ProductCatalog.Infrastructure.EfCore.MediatR.Pipelines;
using DDDEfCore.ProductCatalog.Services.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.MediatR;
internal static class MediatRHostExtension
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        var assembly = typeof(ITransactionCommand<>).Assembly;

        services.AddMediatR(mediatR =>
        {
            mediatR
                .RegisterServicesFromAssembly(assembly)
                .AddOpenBehavior(typeof(RequestValidatorPipelineBehavior<,>))
                .AddOpenBehavior(typeof(CommandTransactionPipelineBehavior<,>));
        });

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
