using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddEfCoreSqlServerDb(configuration)
            .AddCustomMediatR();

        return services;
    }

    public static IServiceCollection AddEfCoreSqlServerDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbContext, ProductCatalogDbContext>((serviceProvider, dbContextOptions) =>
        {
            dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultDb"), opts =>
            {
                opts.EnableRetryOnFailure(maxRetryCount: 3);
            });
        });

        services
            .AddScoped(typeof(IRepository<,>), typeof(DefaultRepositoryAsync<,>))
            .AddScoped<IRepositoryFactory, DefaultRepositoryFactory>();

        return services;
    }
}
