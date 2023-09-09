using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory;

public class TestCatalogCategoryBehaviors : TestEfCoreBase
{
    public TestCatalogCategoryBehaviors(TestEfCoreCollectionFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    private Catalog Catalog = default!;
    private Category Category = default!;
    private Product Product = default!;
    private CatalogCategory CatalogCategory = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());
        this.Product = Product.Create(this._fixture.Create<string>());
        this.Catalog = Catalog.Create(this._fixture.Create<string>());

        this.CatalogCategory =
            this.Catalog.AddCategory(this.Category.Id, this._fixture.Create<string>());

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(this.Category);
            dbContext.Add(this.Product);
            dbContext.Add(this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    private async Task DoActionWithCatalogCategory(Action<CatalogCategory> action)
    {
        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var catalogCategory = await dbContext.Set<Catalog>()
                            .Where(_ => _ == this.Catalog)
                            .SelectMany(_ => _.Categories)
                            .FirstOrDefaultAsync(_ => _ == this.CatalogCategory);

            action(catalogCategory);

            await dbContext.SaveChangesAsync();
        });
    }

    private async Task DoAssertForCatalogCategory(Action<CatalogCategory> action)
    {
        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var catalogCategory = await dbContext.Set<Catalog>()
                            .Where(_ => _ == this.Catalog)
                            .SelectMany(_ => _.Categories)
                            .FirstOrDefaultAsync(_ => _ == this.CatalogCategory);

            action(catalogCategory);
        });
    }

    #region Self Behaviors

    [Theory(DisplayName = "CatalogCategory Change DisplayName Successfully")]
    [AutoData]
    public async Task CatalogCategory_Change_DisplayName_Successfully(string catalogCategoryDisplayName)
    {
        await this.DoActionWithCatalogCategory(catalogCategory =>
        {
            catalogCategory.ChangeDisplayName(catalogCategoryDisplayName);
        });

        await this.DoAssertForCatalogCategory(catalogCategory =>
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

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(product);
            await dbContext.SaveChangesAsync();
        });

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var catalogs = dbContext.Set<Catalog>();

            var query =
                    from c in catalogs
                    from c1 in c.Categories
                    where c.Id == this.Catalog.Id && c1.Id == this.CatalogCategory.Id
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

            this._output.WriteLine(query.ToQueryString());

            var result = await query.FirstOrDefaultAsync();

            result.ShouldNotBeNull();

            var catalog = result.Catalog;
            var catalogCategory = result.CatalogCategory;
            var catalogProduct = catalogCategory.CreateCatalogProduct(product.Id, catalogProductDisplayName);

            await dbContext.SaveChangesAsync();
        });

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var query = dbContext.Set<Catalog>()
                        .Where(_ => _.Id == this.Catalog.Id)
                        .SelectMany(_ => _.Categories)
                        .Where(_ => _.Id == this.CatalogCategory.Id)
                        .SelectMany(_ => _.Products)
                        .Where(_ => _.ProductId == product.Id);

            this._output.WriteLine(query.ToQueryString());

            var catalogProduct = await query.FirstOrDefaultAsync();

            catalogProduct.ShouldNotBeNull();
        });
    }

    [Fact(DisplayName = "CatalogCategory Remove CatalogProduct Successfully")]
    public async Task CatalogCategory_Remove_CatalogProduct_Successfully()
    {
        var product = Product.Create("Product");

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(product);
            await dbContext.SaveChangesAsync();
        });

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var catalogs = dbContext.Set<Catalog>();

            var query =
                    from c in catalogs
                    from c1 in c.Categories
                    where c.Id == this.Catalog.Id && c1.Id == this.CatalogCategory.Id
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

            this._output.WriteLine(query.ToQueryString());

            var result = await query.FirstOrDefaultAsync();

            result.ShouldNotBeNull();

            var catalog = result.Catalog;
            var catalogCategory = result.CatalogCategory;
            var catalogProduct = catalogCategory.CreateCatalogProduct(product.Id, "Catalog-Product");

            await dbContext.SaveChangesAsync();
        });

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var query = dbContext.Set<Catalog>()
                        .Where(_ => _.Id == this.Catalog.Id)
                        .SelectMany(_ => _.Categories)
                        .Where(_ => _.Id == this.CatalogCategory.Id)
                        .SelectMany(_ => _.Products)
                        .Where(_ => _.ProductId == product.Id);

            var result = await query.ExecuteDeleteAsync();

            result.ShouldBe(1);
        });

        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var query = dbContext.Set<Catalog>()
                        .Where(_ => _.Id == this.Catalog.Id)
                        .SelectMany(_ => _.Categories)
                        .Where(_ => _.Id == this.CatalogCategory.Id)
                        .SelectMany(_ => _.Products)
                        .Where(_ => _.ProductId == product.Id);

            this._output.WriteLine(query.ToQueryString());

            var catalogProduct = await query.FirstOrDefaultAsync();

            catalogProduct.ShouldBeNull();
        });
    }
    #endregion
}
