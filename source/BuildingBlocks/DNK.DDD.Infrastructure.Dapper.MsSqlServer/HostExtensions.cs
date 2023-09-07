using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace DNK.DDD.Infrastructure.Dapper.MsSqlServer;
public static class HostExtensions
{
    public static WebApplicationBuilder AddCustomDapper(this WebApplicationBuilder builder, string nameOfConnectionString)
    {
        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString(nameOfConnectionString)!;

        var services = builder.Services;

        services.AddScoped(typeof(IDbConnectionFactory), serviceProvider =>
        {
            return new MsSqlServerConnectionFactory(connectionString);
        });

        services.AddScoped(typeof(IDbConnection), serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
            return factory.GetConnection();
        });

        return builder;
    }
}
