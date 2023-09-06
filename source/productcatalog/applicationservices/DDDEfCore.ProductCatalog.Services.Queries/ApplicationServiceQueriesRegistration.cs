using Dapper;
using DDDEfCore.ProductCatalog.Core.DomainModels;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Queries
{
    public static class ApplicationServiceQueriesRegistration
    {
        public static IServiceCollection AddApplicationQueries(this IServiceCollection services)
        {
            services.AddScoped(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultDb");
                return new SqlServerDbConnectionFactory(connectionString);
            });


            StronglyTypedIdTypeDescriptor.AddStronglyTypedIdConverter((idType) =>
            {
                var idTypeHandler = typeof(StronglyTypedIdMapper<>).MakeGenericType(idType);
                var idTypeHandlerInstance = (SqlMapper.ITypeHandler)Activator.CreateInstance(idTypeHandler);
                SqlMapper.AddTypeHandler(idType, idTypeHandlerInstance);
            });

            return services;
        }

        
    }
}
