using DDDEF.Infrastructure.EFCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DDDEF.Tests;
public class WebApplicationFactoryForTest(string connectionString) : WebApplicationFactory<Program>
{
    private readonly string _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        var settingsInMemory = new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = this._connectionString
        };

        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settingsInMemory)
                .Build();

        builder
            .UseEnvironment("Integration-Test")
            .UseConfiguration(configuration)
            .UseTestServer()
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.AddInMemoryCollection(settingsInMemory);
            })
            .ConfigureServices(services =>
            {
                services.RemoveAll<IHostedService>();
            })
            .ConfigureTestServices(services =>
            {
                //TODO: override services for testing only
            });
    }

    public async Task ExecDbContextAsync(Func<ProjectManagementContext, Task> func)
    {
        using var scope = this.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
        await func(dbContext);
    }

    public async Task RunDbMigration()
    {
        await this.ExecDbContextAsync(async db =>
        {
            var database = db.Database;
            var migrations = await database.GetPendingMigrationsAsync();
            if (!migrations.Any())
            {
                return;
            }

            await database.MigrateAsync();
        });
        
    }

}
