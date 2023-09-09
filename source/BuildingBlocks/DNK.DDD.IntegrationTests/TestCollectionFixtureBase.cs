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
                .WithExposedPort(14333)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("P@ssw0rd-01")
                .Build();
    }

    public async Task DisposeAsync()
    {
        await this._container.DisposeAsync().ConfigureAwait(false);
    }

    public async Task InitializeAsync()
    {
        await this._container.StartAsync();

        this.Factory = (TWebApplicationFactory)Activator.CreateInstance(typeof(TWebApplicationFactory), this._container.GetConnectionString())!;

        await this.Factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            var database = dbContext.Database;

            await this.ApplyMigrations(database);
        });
    }

    protected abstract Task ApplyMigrations(DatabaseFacade database);
}
