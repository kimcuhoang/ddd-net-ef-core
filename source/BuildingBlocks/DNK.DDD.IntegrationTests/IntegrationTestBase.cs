using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace DNK.DDD.IntegrationTests;
public abstract class IntegrationTestBase<TTestCollectionFixture, TWebApplicationFactory, TProgram>(TTestCollectionFixture testCollectionFixture, ITestOutputHelper output) : IAsyncLifetime
        where TTestCollectionFixture : TestCollectionFixtureBase<TWebApplicationFactory, TProgram>
        where TWebApplicationFactory: WebApplicationFactoryBase<TProgram>
        where TProgram : class
{
    protected TWebApplicationFactory Factory { get; } = testCollectionFixture.Factory;
    protected readonly ITestOutputHelper _output = output;
    protected readonly IFixture _fixture = new Fixture();

    protected async Task ExecuteTransactionDbContext(Func<DbContext, Task> func)
    {
        await this.Factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // Achieving atomicity
                await using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    await func.Invoke(dbContext);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        });
    }

    protected async Task ExecuteDbContextAsync(Func<DbContext, Task> func)
    {
        await this.Factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            await func.Invoke(dbContext);
        });
    }

    protected async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
        await this.Factory.ExecuteServiceAsync(func);
    }

    protected async Task ExecuteHttpClientAsync(Func<HttpClient, Task> func)
    {
        using var httpClient = this.Factory.CreateClient();
        await func(httpClient);
    }

    protected StringContent ConvertRequestToStringContent(object request)
    {
        var jsonSerializerSettings = this.Factory.JsonSerializerSettings;

        var requestAsJson = JsonSerializer.Serialize(request, jsonSerializerSettings);

        return new StringContent(requestAsJson, Encoding.UTF8, "application/json");
    }

    protected async ValueTask<TModel?> ParseResponse<TModel>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        this._output.WriteLine(content);

        return JsonSerializer.Deserialize<TModel>(content, this.Factory.JsonSerializerSettings);
    }

    public virtual async Task DisposeAsync()
    {
        await Task.Yield();
    }

    public virtual async Task InitializeAsync()
    {
        await Task.Yield();
    }
}