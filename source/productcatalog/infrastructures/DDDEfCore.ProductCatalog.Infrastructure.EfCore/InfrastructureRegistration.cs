using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddEfCoreSqlServerDb(this IServiceCollection services)
        {
            var svcProvider = services.BuildServiceProvider();

            services.Replace(
                ServiceDescriptor.Scoped<
                    IDbConnStringFactory,
                    SqlServerConnectionStringFactory>());

            services.Replace(
                ServiceDescriptor.Scoped<
                    IExtendDbContextOptionsBuilder,
                    SqlServerDbContextOptionsBuilder>());

            services.AddDbContext<DbContext, ProductCatalogDbContext>((sp, o) =>
            {
                var extendOptionsBuilder = sp.GetRequiredService<IExtendDbContextOptionsBuilder>();
                var connStringFactory = sp.GetRequiredService<IDbConnStringFactory>();
                extendOptionsBuilder.Extend(o, connStringFactory, string.Empty);
            });

            services.Replace(
                ServiceDescriptor.Scoped<
                    IRepositoryFactory,
                    DefaultRepositoryFactory>());

            services.Replace(
                ServiceDescriptor.Scoped<
                    DatabaseMigration,
                    SqlServerDatabaseMigration>());

            return services;
        }
    }
}
