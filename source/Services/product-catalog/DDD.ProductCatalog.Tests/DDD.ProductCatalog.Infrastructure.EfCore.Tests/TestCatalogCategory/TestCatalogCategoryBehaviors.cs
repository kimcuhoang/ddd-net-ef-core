using AutoFixture.Xunit2;
using DDD.ProductCatalog.Infrastructure.EfCore.Tests;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory;

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
        var product = Product.Create("Product");

        await this._fixture.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(product);
            await dbContext.SaveChangesAsync();
        });

        await this._fixture.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            var catalogs = dbContext.Set<Catalog>();

            var query =
                    from c in catalogs
                    from c1 in c.Categories
                    where c.Id == this._fixture.Catalog.Id && c1.Id == this._fixture.CatalogCategory.Id
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

            this._testOutput.WriteLine(query.ToQueryString());

            var result = await query.FirstOrDefaultAsync();

            result.ShouldNotBeNull();

            var catalog = result.Catalog;
            var catalogCategory = result.CatalogCategory;
            var catalogProduct = catalogCategory.CreateCatalogProduct(product.Id, catalogProductDisplayName);

            await dbContext.SaveChangesAsync();
        });

        await this._fixture.ExecuteDbContextAsync(async dbContext =>
        {
            var query = dbContext.Set<Catalog>()
                        .Where(_ => _.Id == this._fixture.Catalog.Id)
                        .SelectMany(_ => _.Categories)
                        .Where(_ => _.Id == this._fixture.CatalogCategory.Id)
                        .SelectMany(_ => _.Products)
                        .Where(_ => _.ProductId == product.Id);

            this._testOutput.WriteLine(query.ToQueryString());

            var catalogProduct = await query.FirstOrDefaultAsync();

            catalogProduct.ShouldNotBeNull();
        });
    }

    [Fact(DisplayName = "CatalogCategory Remove CatalogProduct Successfully")]
    public async Task CatalogCategory_Remove_CatalogProduct_Successfully()
    {
        var product = Product.Create("Product");

        await this._fixture.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(product);
            await dbContext.SaveChangesAsync();
        });

        await this._fixture.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            var catalogs = dbContext.Set<Catalog>();

            var query =
                    from c in catalogs
                    from c1 in c.Categories
                    where c.Id == this._fixture.Catalog.Id && c1.Id == this._fixture.CatalogCategory.Id
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

            this._testOutput.WriteLine(query.ToQueryString());

            var result = await query.FirstOrDefaultAsync();

            result.ShouldNotBeNull();

            var catalog = result.Catalog;
            var catalogCategory = result.CatalogCategory;
            var catalogProduct = catalogCategory.CreateCatalogProduct(product.Id, "Catalog-Product");

            await dbContext.SaveChangesAsync();
        });

        await this._fixture.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            var query = dbContext.Set<Catalog>()
                        .Where(_ => _.Id == this._fixture.Catalog.Id)
                        .SelectMany(_ => _.Categories)
                        .Where(_ => _.Id == this._fixture.CatalogCategory.Id)
                        .SelectMany(_ => _.Products)
                        .Where(_ => _.ProductId == product.Id);

            var result = await query.ExecuteDeleteAsync();

            result.ShouldBe(1);
        });

        await this._fixture.ExecuteDbContextAsync(async dbContext =>
        {
            var query = dbContext.Set<Catalog>()
                        .Where(_ => _.Id == this._fixture.Catalog.Id)
                        .SelectMany(_ => _.Categories)
                        .Where(_ => _.Id == this._fixture.CatalogCategory.Id)
                        .SelectMany(_ => _.Products)
                        .Where(_ => _.ProductId == product.Id);

            this._testOutput.WriteLine(query.ToQueryString());

            var catalogProduct = await query.FirstOrDefaultAsync();

            catalogProduct.ShouldBeNull();
        });
    }
    #endregion
}
