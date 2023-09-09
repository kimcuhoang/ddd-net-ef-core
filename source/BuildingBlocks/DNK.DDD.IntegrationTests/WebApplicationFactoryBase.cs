using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;

namespace DNK.DDD.IntegrationTests;
public abstract class WebApplicationFactoryBase<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _connectionString;

    protected WebApplicationFactoryBase(string connectionString)
    {
        this._connectionString = connectionString;
    }

    protected virtual Dictionary<string, string?> InMemorySettings
    {
        get
        {
            return new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultDb"] = this._connectionString
            };
        }
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(this.InMemorySettings)
                .Build();

        builder
            .UseEnvironment("Integration-Test")
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(configuration)
            .UseTestServer()
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.AddInMemoryCollection(this.InMemorySettings);
            })
            .ConfigureServices(services =>
            {
                services.RemoveAll<IHostedService>();
            })
            .ConfigureTestServices(this.ConfigureTestServices);
    }

    public JsonSerializerOptions JsonSerializerSettings
    {
        get
        {
            var jsonSettings = this.Services.GetRequiredService<IOptions<JsonOptions>>().Value;
            return jsonSettings?.SerializerOptions!;
        }
    }

    public async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
        using var scope = this.Services.CreateAsyncScope();
        await func.Invoke(scope.ServiceProvider);
    }
}
