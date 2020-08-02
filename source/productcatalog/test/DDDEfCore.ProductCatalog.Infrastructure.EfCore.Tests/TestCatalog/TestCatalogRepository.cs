using AutoFixture.Xunit2;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogRepository : IClassFixture<TestCatalogFixture>
    {
        private readonly TestCatalogFixture _testFixture;

        public TestCatalogRepository(TestCatalogFixture testFixture)
        {
            this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));
        }

        [Fact(DisplayName = "Create Catalog with Chain of Catalog-Categories Successfully")]
        public async Task Create_Catalog_With_Chain_Of_CatalogCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldNotBeNull();
                catalog.Equals(this._testFixture.Catalog).ShouldBeTrue();
                catalog
                    .Categories
                    .Except(this._testFixture.CatalogCategories)
                    .Any()
                    .ShouldBeFalse();

                var roots = catalog.FindCatalogCategoryRoots();
                roots.ShouldHaveSingleItem();

                var descendantsOfLv1 = catalog.GetDescendantsOfCatalogCategory(this._testFixture.CatalogCategoryLv1);
                descendantsOfLv1
                    .Except(this._testFixture.CatalogCategories)
                    .Any()
                    .ShouldBeFalse();

                var descendantsOfLv2 = catalog.GetDescendantsOfCatalogCategory(this._testFixture.CatalogCategoryLv2);
                descendantsOfLv2
                    .Except(this._testFixture.CatalogCategories.Where(x => x != this._testFixture.CatalogCategoryLv1))
                    .Any()
                    .ShouldBeFalse();

                var descendantsOfLv3 = catalog.GetDescendantsOfCatalogCategory(this._testFixture.CatalogCategoryLv3);
                descendantsOfLv3
                    .Except(this._testFixture.Catalog.Categories.Where(x => x != this._testFixture.CatalogCategoryLv1 && x != this._testFixture.CatalogCategoryLv2))
                    .Any().ShouldBeFalse();
            });
        }

        [Fact(DisplayName = "Catalog Should Remove Chain of CatalogCategories Successfully")]
        public async Task Catalog_Should_Remove_Chain_of_CatalogCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.DoActionWithCatalog(catalog =>
            {
                var catalogCategoryLv1 = catalog.FindCatalogCategoryRoots().FirstOrDefault();
                catalogCategoryLv1.ShouldBe(this._testFixture.CatalogCategoryLv1);

                catalog.RemoveCatalogCategoryWithDescendants(catalogCategoryLv1);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldNotBeNull();
                catalog.Categories.ShouldBeEmpty();
            });
        }

        [Fact(DisplayName = "Remove Catalog Within CatalogCategories Successfully")]
        public async Task RemoveCatalog_Within_CatalogCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.RepositoryExecute<Catalog, CatalogId>(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.Id == this._testFixture.Catalog.Id,
                        x => x.Include(y => y.Categories));

                await repository.RemoveAsync(catalog);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldBeNull();
            });
        }

        [Theory(DisplayName = "Create 2 CatalogCategory Successfully")]
        [AutoData]
        public async Task Create_2_CatalogCategory_Successfully(string nameOfCategory1, string nameofCategory2)
        {
            await this._testFixture.InitData();

            var category1 = Category.Create(nameOfCategory1);
            var category2 = Category.Create(nameofCategory2);

            CatalogCategory catalogCategory1 = null;
            CatalogCategory catalogCategory2 = null;

            await this._testFixture.SeedingData<Category,CategoryId>(category1, category2);

            await this._testFixture.DoActionWithCatalog(catalog =>
            {
                catalogCategory1 =
                    catalog.AddCategory(category1.Id, category1.DisplayName);

                catalogCategory2 =
                    catalog.AddCategory(category2.Id, category2.DisplayName);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.Categories.ShouldContain(catalogCategory1);
                catalog.Categories.ShouldContain(catalogCategory2);
            });

        }
    }
}
