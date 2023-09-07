using DNK.DDD.Infrastructure.EntityFrameworkCore.MediatR.Pipelines;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore.MediatR;
public static class MediatRHostExtensions
{
    public static WebApplicationBuilder AddCustomMediatR(this WebApplicationBuilder builder, params Assembly[] assemblies)
    {
        var services = builder.Services;

        services.AddMediatR(mediatR =>
        {
            mediatR
                .RegisterServicesFromAssemblies(assemblies)
                .AddOpenBehavior(typeof(ValidateRequestPipelineBehavior<,>))
                .AddOpenBehavior(typeof(CommitTransactionCommandPipelineBehavior<,>));
        });

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssemblies(assemblies);

        return builder;
    }
}
