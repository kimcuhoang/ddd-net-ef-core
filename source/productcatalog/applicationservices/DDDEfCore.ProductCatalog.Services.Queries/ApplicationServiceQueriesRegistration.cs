using DDDEfCore.ProductCatalog.Services.Queries.Db;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Queries
{
    public static class ApplicationServiceQueriesRegistration
    {
        public static IServiceCollection AddApplicationQueries(this IServiceCollection services)
        {
            services.AddScoped<SqlServerDbConnectionFactory>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultDb");
                return new SqlServerDbConnectionFactory(connectionString);
            });
            services.AddMediatR(typeof(SqlServerDbConnectionFactory).Assembly);
            services.AddValidatorsFromAssembly(typeof(SqlServerDbConnectionFactory).Assembly);
            return services;
        }
    }
}
