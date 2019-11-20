using System.Reflection;
using DDDEfCore.ProductCatalog.Core.DomainModels;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore;
using DDDEfCore.ProductCatalog.Services.Commands.Infrastructure.PipelineBehaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Commands.Infrastructure
{
    public static class ApplicationServiceCommandsRegistration
    {
        public static IServiceCollection AddApplicationCommands(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddEfCoreSqlServerDb();
            services.AddMediatR(assembly);
            services.AddValidatorsFromAssembly(assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EndRequestPipelineBehavior<,>));

            StronglyTypedIdTypeDescriptor.AddStronglyTypedIdConverter();

            return services;
        }
    }
}
