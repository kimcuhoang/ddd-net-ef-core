using DDD.ProductCatalog.WebApi.Infrastructures.HostedServices;
using DNK.DDD.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.ProductCatalog.WebApi.Tests;

public class DefaultWebApplicationFactory : WebApplicationFactoryBase<Program>
{
    public DefaultWebApplicationFactory(string connectionString) : base(connectionString)
    {
    }

    protected override Dictionary<string, string?> InMemorySettings
    {
        get
        {
            var inMemorySettings = base.InMemorySettings;

            return inMemorySettings;
        }
    }

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddHostedService<DbMigratorHostedService>();
    }
}
