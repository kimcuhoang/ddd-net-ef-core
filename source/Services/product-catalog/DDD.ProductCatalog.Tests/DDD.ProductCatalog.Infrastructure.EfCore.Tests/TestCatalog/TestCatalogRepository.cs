using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog;

public class TestCatalogRepository : TestEfCoreBase
{
    public TestCatalogRepository(TestEfCoreCollectionFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    private Catalog Catalog = default!;

    private Category CategoryLv1 = default!;
    private Category CategoryLv2 = default!;
    private Category CategoryLv3 = default!;

    private CatalogCategory CatalogCategoryLv1 = default!;
    private CatalogCategory CatalogCategoryLv2 = default!;
    private CatalogCategory CatalogCategoryLv3 = default!;

    public IEnumerable<CatalogCategory> CatalogCategories => new List<CatalogCategory>
    {
        this.CatalogCategoryLv1,
        this.CatalogCategoryLv2,
        this.CatalogCategoryLv3
    };

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.CategoryLv1 = Category.Create(this._fixture.Create<string>());
        this.CategoryLv2 = Category.Create(this._fixture.Create<string>());
        this.CategoryLv3 = Category.Create(this._fixture.Create<string>());

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.AddRange(this.CategoryLv1, this.CategoryLv2, this.CategoryLv3);
            await dbContext.SaveChangesAsync();
        });
    }

    private async Task InitData()
    {
        this.Catalog = Catalog.Create(this._fixture.Create<string>());

        this.CatalogCategoryLv1 =
            this.Catalog.AddCategory(this.CategoryLv1.Id, this._fixture.Create<string>());
        this.CatalogCategoryLv2 =
            this.Catalog.AddCategory(this.CategoryLv2.Id, this._fixture.Create<string>(), this.CatalogCategoryLv1);
        this.CatalogCategoryLv3 =
            this.Catalog.AddCategory(this.CategoryLv3.Id, this._fixture.Create<string>(), this.CatalogCategoryLv2);

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.Add(this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    private async Task DoAssert(Action<Catalog> assertFor)
    {
        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var queryCatalog = await dbContext.Set<Catalog>().Include(_ => _.Categories)
                                .Where(_ => _.Id == this.Catalog.Id)
                                .ToListAsync();

            var catalog = queryCatalog.FirstOrDefault();

            assertFor(catalog);
        });
    }

    private async Task DoActionWithCatalog(Action<Catalog> action)
    {
        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            var queryCatalog = await dbContext.Set<Catalog>().Include(_ => _.Categories)
                                .Where(_ => _.Id == this.Catalog.Id)
                                .ToListAsync();

            var catalog = queryCatalog.FirstOrDefault();

            action(catalog);

            await dbContext.SaveChangesAsync();
        });
    }

    [Fact(DisplayName = "Create Catalog with Chain of Catalog-Categories Successfully")]
    public async Task Create_Catalog_With_Chain_Of_CatalogCategories_Successfully()
    {
        await this.InitData();

        await this.DoAssert(catalog =>
        {
            catalog.ShouldNotBeNull();
            catalog.Equals(this.Catalog).ShouldBeTrue();
            catalog
                .Categories
                .Except(this.CatalogCategories)
                .Any()
                .ShouldBeFalse();

            var roots = catalog.FindCatalogCategoryRoots();
            roots.ShouldHaveSingleItem();

            var descendantsOfLv1 = catalog.GetDescendantsOfCatalogCategory(this.CatalogCategoryLv1);
            descendantsOfLv1
                .Except(this.CatalogCategories)
                .Any()
                .ShouldBeFalse();

            var descendantsOfLv2 = catalog.GetDescendantsOfCatalogCategory(this.CatalogCategoryLv2);
            descendantsOfLv2
                .Except(this.CatalogCategories.Where(x => x != this.CatalogCategoryLv1))
                .Any()
                .ShouldBeFalse();

            var descendantsOfLv3 = catalog.GetDescendantsOfCatalogCategory(this.CatalogCategoryLv3);
            descendantsOfLv3
                .Except(this.Catalog.Categories.Where(x => x != this.CatalogCategoryLv1 && x != this.CatalogCategoryLv2))
                .Any().ShouldBeFalse();
        });
    }

    [Fact(DisplayName = "Catalog Should Remove Chain of CatalogCategories Successfully")]
    public async Task Catalog_Should_Remove_Chain_of_CatalogCategories_Successfully()
    {
        await this.InitData();

        await this.DoActionWithCatalog(catalog =>
        {
            var catalogCategoryLv1 = catalog.FindCatalogCategoryRoots().FirstOrDefault();

            catalogCategoryLv1.ShouldNotBeNull();

            catalogCategoryLv1.ShouldBe(this.CatalogCategoryLv1);

            catalog.RemoveCatalogCategoryWithDescendants(catalogCategoryLv1);
        });

        await this.DoAssert(catalog =>
        {
            catalog.ShouldNotBeNull();
            catalog.Categories.ShouldBeEmpty();
        });
    }

    [Theory(DisplayName = "Create 2 CatalogCategory Successfully")]
    [AutoData]
    public async Task Create_2_CatalogCategory_Successfully(string nameOfCategory1, string nameofCategory2)
    {
        await this.InitData();

        var category1 = Category.Create(nameOfCategory1);
        var category2 = Category.Create(nameofCategory2);

        CatalogCategory? catalogCategory1 = default;
        CatalogCategory? catalogCategory2 = default;

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.AddRange(category1, category2);
            await dbContext.SaveChangesAsync();
        });

        await this.DoActionWithCatalog(catalog =>
        {
            catalogCategory1 = catalog.AddCategory(category1.Id, category1.DisplayName);

            catalogCategory2 = catalog.AddCategory(category2.Id, category2.DisplayName);
        });

        await this.DoAssert(catalog =>
        {
            catalog.Categories.ShouldContain(catalogCategory1);
            catalog.Categories.ShouldContain(catalogCategory2);
        });
    }
}
