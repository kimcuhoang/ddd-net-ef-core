using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace DNK.DDD.IntegrationTests;
public abstract class IntegrationTestBase<TTestFixture, TWebApplicationFactory, TProgram> : IAsyncLifetime, IClassFixture<TTestFixture>
        where TTestFixture: TestFixtureBase<TWebApplicationFactory, TProgram>
        where TWebApplicationFactory: WebApplicationFactoryBase<TProgram>
        where TProgram : class
{
    protected readonly TTestFixture _testFixture;

    protected TWebApplicationFactory _factory => this._testFixture.Factory;
    protected readonly ITestOutputHelper _output;
    protected readonly IFixture _fixture;

    protected IntegrationTestBase(TTestFixture testFixture, ITestOutputHelper output)
    {
        this._testFixture = testFixture;
        this._output = output;
        this._fixture = new Fixture();
    }

    protected async Task ExecuteTransactionDbContext(Func<DbContext, Task> func)
    {
        await this._factory.ExecuteServiceAsync(async serviceProvider =>
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
        await this._factory.ExecuteServiceAsync(async serviceProvider =>
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            await func.Invoke(dbContext);
        });
    }

    protected async Task ExecuteServiceAsync(Func<IServiceProvider, Task> func)
    {
        await this._factory.ExecuteServiceAsync(func);
    }

    protected async Task ExecuteHttpClientAsync(Func<HttpClient, Task> func)
    {
        using var httpClient = this._factory.CreateClient();
        await func(httpClient);
    }

    protected StringContent ConvertRequestToStringContent(object request)
    {
        var jsonSerializerSettings = this._factory.JsonSerializerSettings;

        var requestAsJson = JsonSerializer.Serialize(request, jsonSerializerSettings);

        return new StringContent(requestAsJson, Encoding.UTF8, "application/json");
    }

    protected async ValueTask<TModel?> ParseResponse<TModel>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        this._output.WriteLine(content);

        return JsonSerializer.Deserialize<TModel>(content, this._factory.JsonSerializerSettings);
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