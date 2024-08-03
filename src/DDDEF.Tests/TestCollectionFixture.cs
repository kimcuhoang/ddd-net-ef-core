using DDDEF.Tests;
using System.Diagnostics;
using Testcontainers.MsSql;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

public class TestCollectionFixture : IAsyncLifetime
{
    public WebApplicationFactoryForTest WebApplicationFactory { get; private set; } = default!;

    public MsSqlContainer Container { get; private set; } = default!;


    /// <summary>
    /// Override the GetConnectionString from MsSqlBuilder
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    //private string GetConnectionString(MsSqlContainer container, string thePassword)
    //{
    //    var properties = new Dictionary<string, string>
    //    {
    //        { "Server", $"{container.Hostname},{container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort)}"},
    //        { "Database", MsSqlBuilder.DefaultDatabase },
    //        { "User Id", MsSqlBuilder.DefaultUsername },
    //        { "Password", MsSqlBuilder.DefaultPassword },
    //        { "Encrypt", bool.FalseString },
    //        { "MultipleActiveResultSets", bool.TrueString }
    //    };
    //    return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    //}

    public async Task DisposeAsync()
    {
        await this.Container.DisposeAsync();
        Debug.WriteLine($"{nameof(TestCollectionFixture)} {nameof(DisposeAsync)}");
    }

    public async Task InitializeAsync()
    {
        await new MsSqlBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithHostname("test")
                .WithPortBinding(1433, assignRandomHostPort: true)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithStartupCallback(async(c, cancellationToken) =>
                {
                    Debug.WriteLine($"{nameof(TestCollectionFixture)} - after container started");

                    this.Container = c;
                    this.WebApplicationFactory = new WebApplicationFactoryForTest(c.GetConnectionString());
                    await Task.Yield();
                })
                .Build()
                .StartAsync();

        await this.WebApplicationFactory.RunDbMigration();

        Debug.WriteLine($"{nameof(TestCollectionFixture)} {nameof(InitializeAsync)}");
    }
}