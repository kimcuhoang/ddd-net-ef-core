using DDDEfCore.ProductCatalog.Infrastructure.EfCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands
{
    public static class ApplicationServiceCommandsRegistration
    {
        public static IServiceCollection AddApplicationCommands(this IServiceCollection services)
        {
            services.AddEfCoreSqlServerDb();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
