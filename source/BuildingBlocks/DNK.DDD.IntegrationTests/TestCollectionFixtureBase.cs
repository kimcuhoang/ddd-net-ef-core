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

    protected TestCollectionFixtureBase()
    {
        this._container = new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithPortBinding(1433, assignRandomHostPort: true)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("P@ssw0rd-01")
                .WithStartupCallback(async (container, cancellationToken) =>
                {
                    var msSqlContainer = (MsSqlContainer)container;
                    this.Factory = (TWebApplicationFactory)Activator.CreateInstance(typeof(TWebApplicationFactory), msSqlContainer.GetConnectionString())!;
                })
                .Build();
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
