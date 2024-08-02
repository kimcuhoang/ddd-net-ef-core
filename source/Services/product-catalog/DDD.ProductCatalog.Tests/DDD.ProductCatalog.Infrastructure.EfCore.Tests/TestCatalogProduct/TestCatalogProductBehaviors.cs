using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogProduct;

public class TestCatalogProductBehaviors(TestEfCoreCollectionFixture testFixture, ITestOutputHelper output) : TestEfCoreBase(testFixture, output)
{
    private Catalog Catalog = default!;
    private Category Category = default!;
    private Product Product = default!;
    private CatalogCategory CatalogCategory = default!;
    private CatalogProduct CatalogProduct = default!;


    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());
        this.Product = Product.Create(this._fixture.Create<string>());
        this.Catalog = Catalog.Create(this._fixture.Create<string>());

        this.CatalogCategory =
            this.Catalog.AddCategory(this.Category.Id, this._fixture.Create<string>());

        this.CatalogProduct =
            this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this._fixture.Create<string>());

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(this.Category);
            dbContext.Add(this.Product);
            dbContext.Add(this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    private async Task DoActionWithCatalogProduct(Action<CatalogProduct> action)
    {
        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var catalogProduct = await dbContext.Set<Catalog>().Where(_ => _ == this.Catalog)
                            .SelectMany(_ => _.Categories).Where(_ => _ == this.CatalogCategory)
                            .SelectMany(_ => _.Products).FirstOrDefaultAsync(_ => _ == this.CatalogProduct);

            catalogProduct.ShouldNotBeNull();

            action(catalogProduct);

            await dbContext.SaveChangesAsync();
        });
    }

    private async Task DoAssertForCatalogProduct(Action<CatalogProduct> action)
    {
        await this.ExecuteDbContextAsync(async dbContext =>
        {
            var catalogProduct = await dbContext.Set<Catalog>().Where(_ => _ == this.Catalog)
                            .SelectMany(_ => _.Categories).Where(_ => _ == this.CatalogCategory)
                            .SelectMany(_ => _.Products).FirstOrDefaultAsync(_ => _ == this.CatalogProduct);

            catalogProduct.ShouldNotBeNull();

            action(catalogProduct);
        });
    }

    [Theory(DisplayName = "CatalogProduct Change DisplayName Successfully")]
    [AutoData]
    public async Task CatalogProduct_Change_DisplayName_Successfully(string changeToName)
    {
        await this.DoActionWithCatalogProduct(catalogProduct =>
        {
            catalogProduct.ChangeDisplayName(changeToName);
        });

        await this.DoAssertForCatalogProduct(catalogProduct =>
        {
            catalogProduct.DisplayName.ShouldBe(changeToName);
        });
    }
}
