using DDDEF.Infrastructure.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DDDEF.Infrastructure;

internal static class EFCoreRegistration
{
    public static WebApplicationBuilder AddEFCore(this WebApplicationBuilder builder, string nameOfConnectionString)
    {
        var configuration = builder.Configuration;

        builder.Services.AddDbContextPool<ProjectManagementContext>(c =>
        {
            var connectionString = configuration.GetConnectionString(nameOfConnectionString);
            c.UseSqlServer(connectionString, db =>
            {
                db.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });
        });
        return builder;
    }
}
