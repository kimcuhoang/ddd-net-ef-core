using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace DNK.DDD.IntegrationTests;

public abstract class TestCollectionFixtureBase<TWebApplicationFactory, TProgram> : IAsyncLifetime
        where TWebApplicationFactory : WebApplicationFactoryBase<TProgram>
        where TProgram : class
{
    private readonly MsSqlContainer _container;
    public TWebApplicationFactory Factory { get; private set; } = default!;
    private const string MsSqlPassword = "P@ssw0rd-01";

    protected TestCollectionFixtureBase()
    {
        this._container = new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithPortBinding(1433, assignRandomHostPort: true)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword(MsSqlPassword)
                .WithStartupCallback(async (container, cancellationToken) =>
                {
                    this.Factory = (TWebApplicationFactory)Activator
                                    .CreateInstance(typeof(TWebApplicationFactory), this.GetConnectionString(container))!;
                    await Task.Yield();
                })
                .Build();
    }

    /// <summary>
    /// Override the GetConnectionString from MsSqlBuilder
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    private string GetConnectionString(MsSqlContainer container)
    {
        var properties = new Dictionary<string, string>
        {
            { "Server", container.Hostname + "," + container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort) },
            { "Database", MsSqlBuilder.DefaultDatabase },
            { "User Id", MsSqlBuilder.DefaultUsername },
            { "Password", MsSqlPassword },
            { "Encrypt", bool.FalseString },
            { "MultipleActiveResultSets", bool.TrueString }
        };
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }


    public async Task DisposeAsync()
    {
        await this._container.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await this._container.StartAsync();

        await this.Factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            var database = dbContext.Database;

            await this.ApplyMigrations(database);
        });
    }

    protected abstract Task ApplyMigrations(DatabaseFacade database);
}
