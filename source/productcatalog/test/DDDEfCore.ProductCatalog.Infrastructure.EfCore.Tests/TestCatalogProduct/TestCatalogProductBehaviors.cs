using AutoFixture.Xunit2;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogProduct
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogProductBehaviors : IClassFixture<TestCatalogProductFixture>
    {
        private readonly TestCatalogProductFixture _testFixture;

        public TestCatalogProductBehaviors(TestCatalogProductFixture testFixture)
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Theory(DisplayName = "CatalogProduct Change DisplayName Successfully")]
        [AutoData]
        public async Task CatalogProduct_Change_DisplayName_Successfully(string changeToName)
        {
            await this._testFixture.DoActionWithCatalogProduct(catalogProduct =>
            {
                catalogProduct.ChangeDisplayName(changeToName);
            });

            await this._testFixture.DoAssertForCatalogProduct(catalogProduct =>
            {
                catalogProduct.DisplayName.ShouldBe(changeToName);
            });
        }
    }
}
