using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddEfCoreSqlServerDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbContext, ProductCatalogDbContext>((serviceProvider, dbContextOptions) =>
        {
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultDb"), opts =>
            {
                opts.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });
            
        });

        services.AddScoped<IRepositoryFactory, DefaultRepositoryFactory>();

        return services;
    }
}
