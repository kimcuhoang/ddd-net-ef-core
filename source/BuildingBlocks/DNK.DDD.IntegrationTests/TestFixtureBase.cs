namespace DNK.DDD.IntegrationTests;
public abstract class TestFixtureBase<TWebApplicationFactory, TProgram> : IAsyncLifetime
        where TWebApplicationFactory : WebApplicationFactoryBase<TProgram>
        where TProgram : class
{
    public TWebApplicationFactory Factory { get; }

    protected TestFixtureBase(TWebApplicationFactory factory)
    {
        this.Factory = factory;
    }

    public virtual async Task DisposeAsync()
    {
        await Task.Yield();
    }

    public virtual async Task InitializeAsync()
    {
        await this.Factory.StartTestContainer();
    }
}
