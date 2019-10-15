using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
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

        #region Self Behaviors

        [Theory(DisplayName = "CatalogCategory Change DisplayName Successfully")]
        [AutoData]
        public async Task CatalogCategory_Change_DisplayName_Successfully(string catalogCategoryDisplayName)
        {
            await this._testFixture.InitData();

            await this._testFixture.DoActionWithCatalogCategory(catalogCategory =>
            {
                catalogCategory.ChangeDisplayName(catalogCategoryDisplayName);
            });

            await this._testFixture.DoAssertForCatalogCategory(catalogCategory =>
            {
                catalogCategory.DisplayName.ShouldBe(catalogCategoryDisplayName);
            });
        }

        #endregion

        #region Behaviors With CatalogProduct

        [Theory(DisplayName = "CatalogCategory Create CatalogProduct Successfully")]
        [AutoData]
        public async Task CatalogCategory_Create_CatalogProduct_Successfully(string catalogProductDisplayName)
        {
            await this._testFixture.InitData();

            CatalogProduct catalogProduct = null;

            await this._testFixture.DoActionWithCatalogCategory(catalogCategory =>
            {
                catalogProduct =
                    catalogCategory.CreateCatalogProduct(this._testFixture.Product.ProductId, catalogProductDisplayName);
            });

            await this._testFixture.DoAssertForCatalogCategory(catalogCategory =>
            {
                catalogCategory.Products.ShouldNotBeNull();
                catalogCategory.Products.ShouldHaveSingleItem();
                catalogCategory.Products.ShouldContain(catalogProduct);
            });
        }

        [Fact(DisplayName = "CatalogCategory Remove CatalogProduct Successfully")]
        public async Task CatalogCategory_Remove_CatalogProduct_Successfully()
        {
            await this._testFixture.InitDataFull();

            await this._testFixture.DoActionWithCatalogCategory(catalogCategory =>
            {
                var catalogProduct =
                    catalogCategory.Products.SingleOrDefault(x => x == this._testFixture.CatalogProduct);
                catalogCategory.RemoveCatalogProduct(catalogProduct);
            });

            await this._testFixture.DoAssertForCatalogCategory(catalogCategory =>
            {
                catalogCategory.ShouldNotBeNull();
                catalogCategory.Products.ShouldBeEmpty();
            });
        }
        #endregion
    }
}
