using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogCategory;

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

        await this.RepositoryExecute<Catalog, CatalogId>(async repository =>
        {
            await repository.AddAsync(this.Catalog);
        });
    }

    public async Task DoActionWithCatalogCategory(Action<CatalogCategory> action)
    {
        await this.RepositoryExecute<Catalog, CatalogId>(async repository =>
        {
            var catalog = await repository.FindOneWithIncludeAsync(x => x.Id == this.Catalog.Id,
                    x => x.Include(c => c.Categories).ThenInclude(c => c.Products));

            var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

            action(catalogCategory);

            await repository.UpdateAsync(catalog);
        });
    }

    public async Task DoAssertForCatalogCategory(Action<CatalogCategory> action)
    {
        await this.RepositoryExecute<Catalog, CatalogId>(async repository =>
        {
            var catalog = await repository.FindOneWithIncludeAsync(x => x.Id == this.Catalog.Id,
                    x => x.Include(c => c.Categories).ThenInclude(c => c.Products));

            var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

            action(catalogCategory);
        });
    }
}
