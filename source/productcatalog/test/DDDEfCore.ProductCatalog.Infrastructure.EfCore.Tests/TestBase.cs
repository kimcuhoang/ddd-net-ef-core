using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests;

[Collection(nameof(EfCoreTestCollection))]
public abstract class TestBase<TTestFixture>: IClassFixture<TTestFixture> 
    where TTestFixture: DefaultTestFixture
{
    protected readonly ITestOutputHelper _testOutput;
    protected readonly TTestFixture _fixture;

    protected TestBase(ITestOutputHelper testOutput, TTestFixture fixture)
    {
        this._testOutput = testOutput;
        this._fixture = fixture;
    }
}
