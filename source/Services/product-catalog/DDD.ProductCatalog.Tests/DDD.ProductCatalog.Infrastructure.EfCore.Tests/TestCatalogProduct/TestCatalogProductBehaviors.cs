using AutoFixture.Xunit2;
using DDD.ProductCatalog.Infrastructure.EfCore.Tests;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogProduct;

public class TestCatalogProductBehaviors : TestBase<TestCatalogProductFixture>
{
    public TestCatalogProductBehaviors(ITestOutputHelper testOutput, TestCatalogProductFixture fixture) : base(testOutput, fixture)
    {
    }

    [Theory(DisplayName = "CatalogProduct Change DisplayName Successfully")]
    [AutoData]
    public async Task CatalogProduct_Change_DisplayName_Successfully(string changeToName)
    {
        await this._fixture.DoActionWithCatalogProduct(catalogProduct =>
        {
            catalogProduct.ChangeDisplayName(changeToName);
        });

        await this._fixture.DoAssertForCatalogProduct(catalogProduct =>
        {
            catalogProduct.DisplayName.ShouldBe(changeToName);
        });
    }
}
