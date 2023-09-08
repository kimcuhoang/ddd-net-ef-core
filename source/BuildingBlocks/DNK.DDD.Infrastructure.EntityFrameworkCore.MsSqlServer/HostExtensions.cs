using DNK.DDD.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer;

public static class HostExtensions
{
    public static WebApplicationBuilder AddDbContext<TDbContext>(this WebApplicationBuilder builder, string nameOfConnectionString) where TDbContext: DbContext
    {
        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString(nameOfConnectionString);

        var services = builder.Services;
        var assembly = typeof(TDbContext).Assembly;

        services.AddDbContextPool<DbContext, TDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServer =>
            {
                sqlServer.MigrationsAssembly(assembly.GetName().Name);
                sqlServer.EnableRetryOnFailure(maxRetryCount: 3);
            });
        });

        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        return builder;
    }
}
