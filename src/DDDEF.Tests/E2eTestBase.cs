using Bogus;
using DDDEF.Infrastructure.EFCore;
using Xunit.Abstractions;

namespace DDDEF.Tests;

[Collection(nameof(TestCollection))]
public abstract class E2eTestBase(TestCollectionFixture testCollectionFixture, ITestOutputHelper testOutput) : IAsyncLifetime
{
    protected readonly WebApplicationFactoryForTest WebApplicationFactory = testCollectionFixture.WebApplicationFactory;
    protected readonly ITestOutputHelper TestOutput = testOutput;
    protected readonly Faker Faker = new();

    protected async Task ExecuteDbContextAsync(Func<ProjectManagementContext, Task> func)
    {
        await this.WebApplicationFactory.ExecDbContextAsync(func);
    }


    public virtual Task DisposeAsync() => Task.CompletedTask;
    public virtual Task InitializeAsync() => Task.CompletedTask;
}
