using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog;

public class TestCatalogFixture : DefaultTestFixture
{
    public TestCatalogFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Catalog Catalog { get; private set; }

    public Category CategoryLv1 { get; private set; }
    public Category CategoryLv2 { get; private set; }
    public Category CategoryLv3 { get; private set; }

    public CatalogCategory CatalogCategoryLv1 { get; private set; }
    public CatalogCategory CatalogCategoryLv2 { get; private set; }
    public CatalogCategory CatalogCategoryLv3 { get; private set; }

    public IEnumerable<CatalogCategory> CatalogCategories => new List<CatalogCategory>
    {
        this.CatalogCategoryLv1,
        this.CatalogCategoryLv2,
        this.CatalogCategoryLv3
    };

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.CategoryLv1 = Category.Create(this.Fixture.Create<string>());
        this.CategoryLv2 = Category.Create(this.Fixture.Create<string>());
        this.CategoryLv3 = Category.Create(this.Fixture.Create<string>());

        await this.SeedingData<Category, CategoryId>(this.CategoryLv1, this.CategoryLv2, this.CategoryLv3);
    }

    public async Task InitData()
    {
        this.Catalog = Catalog.Create(this.Fixture.Create<string>());

        this.CatalogCategoryLv1 =
            this.Catalog.AddCategory(this.CategoryLv1.Id, this.Fixture.Create<string>());
        this.CatalogCategoryLv2 =
            this.Catalog.AddCategory(this.CategoryLv2.Id, this.Fixture.Create<string>(), this.CatalogCategoryLv1);
        this.CatalogCategoryLv3 =
            this.Catalog.AddCategory(this.CategoryLv3.Id, this.Fixture.Create<string>(), this.CatalogCategoryLv2);

        await this.RepositoryExecute<Catalog,CatalogId>(async repository =>
        {
            await repository.AddAsync(this.Catalog);
        });
    }

    public async Task DoAssert(Action<Catalog> assertFor)
    {
        await this.RepositoryExecute<Catalog,CatalogId>(async repository =>
        {
            var catalog = await repository
                .FindOneWithIncludeAsync(x => x.Id == this.Catalog.Id,
                    x => x.Include(y => y.Categories));

            assertFor(catalog);
        });
    }

    public async Task DoActionWithCatalog(Action<Catalog> action)
    {
        await this.RepositoryExecute<Catalog,CatalogId>(async repository =>
        {
            var catalog = await repository
                .FindOneWithIncludeAsync(x => x.Id == this.Catalog.Id,
                    x => x.Include(y => y.Categories));

            action(catalog);

            await repository.UpdateAsync(catalog);
        });
    }
}
