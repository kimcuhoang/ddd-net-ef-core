using System.Reflection;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ServiceCollectionExtensions = FluentValidation.ServiceCollectionExtensions;

namespace DDDEfCore.ProductCatalog.Services.Commands
{
    public static class ApplicationServiceCommandsRegistration
    {
        public static IServiceCollection AddApplicationCommands(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddEfCoreSqlServerDb();
            services.AddMediatR(assembly);
            services.AddValidatorsFromAssembly(assembly);
            return services;
        }
    }
}
