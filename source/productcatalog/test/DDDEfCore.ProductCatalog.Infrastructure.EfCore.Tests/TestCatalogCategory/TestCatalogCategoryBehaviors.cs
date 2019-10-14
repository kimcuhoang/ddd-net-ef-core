using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogCategoryBehaviors : IClassFixture<TestCatalogCategoryFixture>
    {
        private TestCatalogCategoryFixture _testFixture;

        public TestCatalogCategoryBehaviors(TestCatalogCategoryFixture testFixture)
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Fact(DisplayName = "Add ProductId to CatalogCategory Successfully")]
        public async Task Add_ProductId_To_CatalogCategory_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.DoAssert(catalog =>
            {
                var category = catalog.FindCatalogCategoryRoots().FirstOrDefault();

                category.Products.ShouldNotBeNull();
                category.Products.ShouldHaveSingleItem();
                category.Products.ShouldContain(this._testFixture.CatalogProduct);
            });
        }
    }
}
