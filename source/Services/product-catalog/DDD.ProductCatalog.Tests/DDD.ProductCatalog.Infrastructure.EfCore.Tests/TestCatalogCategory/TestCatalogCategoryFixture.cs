using AutoFixture;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory;

public class TestCatalogCategoryFixture : DefaultTestFixture
{
    public TestCatalogCategoryFixture(DefaultWebApplicationFactory factory)
        : base(factory)
    {
    }

    public Catalog Catalog { get; private set; }
    public Category Category { get; private set; }
    public Product Product { get; private set; }
    public CatalogCategory CatalogCategory { get; private set; }
    public CatalogProduct CatalogProduct { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this.Fixture.Create<string>());
        await this.SeedingData<Category, CategoryId>(this.Category);

        this.Product = Product.Create(this.Fixture.Create<string>());
        await this.SeedingData<Product, ProductId>(this.Product);

        this.Catalog = Catalog.Create(this.Fixture.Create<string>());

        this.CatalogCategory =
            this.Catalog.AddCategory(this.Category.Id, this.Fixture.Create<string>());

        this.CatalogProduct =
            this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Fixture.Create<string>());

        await this.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            dbContext.Add(this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    public async Task DoActionWithCatalogCategory(Action<CatalogCategory> action)
    {
        await this.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            var catalogCategory = await dbContext.Set<Catalog>()
                            .Where(_ => _ == this.Catalog)
                            .SelectMany(_ => _.Categories)
                            .FirstOrDefaultAsync(_ => _ == this.CatalogCategory);

            action(catalogCategory);

            await dbContext.SaveChangesAsync();
        });
    }

    public async Task DoAssertForCatalogCategory(Action<CatalogCategory> action)
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
}
