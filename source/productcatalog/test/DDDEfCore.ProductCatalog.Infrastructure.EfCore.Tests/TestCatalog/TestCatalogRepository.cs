using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog;

public class TestCatalogRepository : TestBase<TestCatalogFixture>
{
    public TestCatalogRepository(ITestOutputHelper testOutput, TestCatalogFixture fixture) 
        : base(testOutput, fixture)
    {
    }

    [Fact(DisplayName = "Create Catalog with Chain of Catalog-Categories Successfully")]
    public async Task Create_Catalog_With_Chain_Of_CatalogCategories_Successfully()
    {
        await this._fixture.InitData();

        await this._fixture.DoAssert(catalog =>
        {
            catalog.ShouldNotBeNull();
            catalog.Equals(this._fixture.Catalog).ShouldBeTrue();
            catalog
                .Categories
                .Except(this._fixture.CatalogCategories)
                .Any()
                .ShouldBeFalse();

            var roots = catalog.FindCatalogCategoryRoots();
            roots.ShouldHaveSingleItem();

            var descendantsOfLv1 = catalog.GetDescendantsOfCatalogCategory(this._fixture.CatalogCategoryLv1);
            descendantsOfLv1
                .Except(this._fixture.CatalogCategories)
                .Any()
                .ShouldBeFalse();

            var descendantsOfLv2 = catalog.GetDescendantsOfCatalogCategory(this._fixture.CatalogCategoryLv2);
            descendantsOfLv2
                .Except(this._fixture.CatalogCategories.Where(x => x != this._fixture.CatalogCategoryLv1))
                .Any()
                .ShouldBeFalse();

            var descendantsOfLv3 = catalog.GetDescendantsOfCatalogCategory(this._fixture.CatalogCategoryLv3);
            descendantsOfLv3
                .Except(this._fixture.Catalog.Categories.Where(x => x != this._fixture.CatalogCategoryLv1 && x != this._fixture.CatalogCategoryLv2))
                .Any().ShouldBeFalse();
        });
    }

    [Fact(DisplayName = "Catalog Should Remove Chain of CatalogCategories Successfully")]
    public async Task Catalog_Should_Remove_Chain_of_CatalogCategories_Successfully()
    {
        await this._fixture.InitData();

        await this._fixture.DoActionWithCatalog(catalog =>
        {
            var catalogCategoryLv1 = catalog.FindCatalogCategoryRoots().FirstOrDefault();
            catalogCategoryLv1.ShouldBe(this._fixture.CatalogCategoryLv1);

            catalog.RemoveCatalogCategoryWithDescendants(catalogCategoryLv1);
        });

        await this._fixture.DoAssert(catalog =>
        {
            catalog.ShouldNotBeNull();
            catalog.Categories.ShouldBeEmpty();
        });
    }

    [Fact(DisplayName = "Remove Catalog Within CatalogCategories Successfully")]
    public async Task RemoveCatalog_Within_CatalogCategories_Successfully()
    {
        await this._fixture.InitData();

        await this._fixture.RepositoryExecute<Catalog, CatalogId>(async repository =>
        {
            var catalog = await repository.FindOneAsync(_ => _ == this._fixture.Catalog);

            catalog.ShouldNotBeNull();

            repository.Remove(catalog);
        });

        await this._fixture.DoAssert(catalog =>
        {
            catalog.ShouldBeNull();
        });
    }

    [Theory(DisplayName = "Create 2 CatalogCategory Successfully")]
    [AutoData]
    public async Task Create_2_CatalogCategory_Successfully(string nameOfCategory1, string nameofCategory2)
    {
        await this._fixture.InitData();

        var category1 = Category.Create(nameOfCategory1);
        var category2 = Category.Create(nameofCategory2);

        CatalogCategory? catalogCategory1 = default;
        CatalogCategory? catalogCategory2 = default;

        await this._fixture.SeedingData<Category,CategoryId>(category1, category2);

        await this._fixture.DoActionWithCatalog(catalog =>
        {
            catalogCategory1 =
                catalog.AddCategory(category1.Id, category1.DisplayName);

            catalogCategory2 =
                catalog.AddCategory(category2.Id, category2.DisplayName);
        });

        await this._fixture.DoAssert(catalog =>
        {
            catalog.Categories.ShouldContain(catalogCategory1);
            catalog.Categories.ShouldContain(catalogCategory2);
        });

    }
}
