using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogProduct;

public class TestCatalogProductFixture : DefaultTestFixture
{
    public TestCatalogProductFixture(DefaultWebApplicationFactory factory) : base(factory)
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
        await this.SeedingData<Category,CategoryId>(this.Category);

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

    public async Task DoActionWithCatalogProduct(Action<CatalogProduct> action)
    {
        await this.ExecuteTransactionDbContextAsync(async dbContext =>
        {
            var catalogProduct = await dbContext.Set<Catalog>().Where(_ => _ == this.Catalog)
                            .SelectMany(_ => _.Categories).Where(_ => _ == this.CatalogCategory)
                            .SelectMany(_ => _.Products).FirstOrDefaultAsync(_ => _ == this.CatalogProduct);

            catalogProduct.ShouldNotBeNull();

            action(catalogProduct);

            await dbContext.SaveChangesAsync();
        });
    }

    public async Task DoAssertForCatalogProduct(Action<CatalogProduct> action)
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
}
