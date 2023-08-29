using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory;

public class TestCatalogCategoryBehaviors : TestBase<TestCatalogCategoryFixture>
{
    public TestCatalogCategoryBehaviors(ITestOutputHelper testOutput, TestCatalogCategoryFixture fixture) 
        : base(testOutput, fixture)
    {
    }


    #region Self Behaviors

    [Theory(DisplayName = "CatalogCategory Change DisplayName Successfully")]
    [AutoData]
    public async Task CatalogCategory_Change_DisplayName_Successfully(string catalogCategoryDisplayName)
    {
        await this._fixture.DoActionWithCatalogCategory(catalogCategory =>
        {
            catalogCategory.ChangeDisplayName(catalogCategoryDisplayName);
        });

        await this._fixture.DoAssertForCatalogCategory(catalogCategory =>
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
        CatalogProduct catalogProduct = null;

        await this._fixture.DoActionWithCatalogCategory(catalogCategory =>
        {
            catalogProduct =
                catalogCategory.CreateCatalogProduct(this._fixture.Product.Id, catalogProductDisplayName);
        });

        await this._fixture.DoAssertForCatalogCategory(catalogCategory =>
        {
            catalogCategory.Products.ShouldNotBeNull();
            catalogCategory.Products.ShouldHaveSingleItem();
            catalogCategory.Products.ShouldContain(catalogProduct);
        });
    }

    [Fact(DisplayName = "CatalogCategory Remove CatalogProduct Successfully")]
    public async Task CatalogCategory_Remove_CatalogProduct_Successfully()
    {
        await this._fixture.DoActionWithCatalogCategory(catalogCategory =>
        {
            var catalogProduct =
                catalogCategory.Products.SingleOrDefault(x => x == this._fixture.CatalogProduct);
            catalogCategory.RemoveCatalogProduct(catalogProduct);
        });

        await this._fixture.DoAssertForCatalogCategory(catalogCategory =>
        {
            catalogCategory.ShouldNotBeNull();
            catalogCategory.Products.ShouldBeEmpty();
        });
    }
    #endregion
}
