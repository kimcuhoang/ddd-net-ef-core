using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Testcontainers.MsSql;
using Microsoft.Extensions.Configuration;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DDD.ProductCatalog.WebApi.Tests;

[CollectionDefinition(nameof(WebApiTestCollection))]
public class WebApiTestCollection : ICollectionFixture<DefaultWebApplicationFactory>
{
}

public class DefaultWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MsSqlContainer _container;

    public DefaultWebApplicationFactory()
    {
        this._container = new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithExposedPort(14333)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("P@ssw0rd-01")
                .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var settingsInMemory = new Dictionary<string, string>
        {
            ["ConnectionStrings:DefaultDb"] = this._container.GetConnectionString()
        };

        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settingsInMemory)
                .Build();

        builder
            .UseEnvironment("Integration-Test")
            .UseContentRoot(Directory.GetCurrentDirectory())
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
            .ConfigureTestServices(this.ConfigureTestServices);
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
    }

    public async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
        using var scope = this.Services.CreateAsyncScope();
        await func.Invoke(scope.ServiceProvider);
    }

    public JsonSerializerOptions JsonSerializerSettings
    {
        get
        {
            var jsonSettings = this.Services.GetRequiredService<IOptions<JsonOptions>>().Value;
            return jsonSettings?.SerializerOptions;
        }
    }

    public async Task StartContainerAsync() => await this._container.StartAsync();
}
