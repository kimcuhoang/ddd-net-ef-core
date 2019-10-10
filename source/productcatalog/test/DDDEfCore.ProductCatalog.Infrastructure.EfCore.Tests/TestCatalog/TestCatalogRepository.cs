using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
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
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Fact(DisplayName = "Should Create Catalog and Sub Categories Successfully")]
        public async Task ShouldCreateCatalog_WithSubCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldNotBeNull();
                catalog.Equals(this._testFixture.Catalog).ShouldBeTrue();
                catalog.Categories.Count().ShouldBe(3);
                catalog.Categories.ShouldBeAssignableTo<IEnumerable<CatalogCategory>>();

                var subCategoriesLv1 = catalog.Categories.Where(x => x.Parent == null).ToList();
                subCategoriesLv1.ShouldHaveSingleItem();

                var subCategoriesLv2 = subCategoriesLv1.SelectMany(x => x.SubCategories).ToList();
                subCategoriesLv2.ShouldHaveSingleItem();

                var subCategoriesLv3 = subCategoriesLv2.SelectMany(x => x.SubCategories).ToList();
                subCategoriesLv3.ShouldHaveSingleItem();
            });
        }

        [Fact(DisplayName = "Should Update Catalog by Removing Sub Categories Successfully")]
        public async Task ShouldUpdateCatalog_ByRemovingSubCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this._testFixture.Catalog.CatalogId,
                        x => x.Include(y => y.Categories));

                catalog.RemoveCategoryWithDescendants(this._testFixture.CategoryLv1.CategoryId);

                await repository.UpdateAsync(catalog);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldNotBeNull();
                catalog.Equals(this._testFixture.Catalog).ShouldBeTrue();
                catalog.Categories.ShouldBeEmpty();
            });
        }

        [Fact(DisplayName = "Should Remove Catalog with Sub Categories Successfully")]
        public async Task ShouldRemoveCatalog_AndSubCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this._testFixture.Catalog.CatalogId,
                        x => x.Include(y => y.Categories));

                await repository.RemoveAsync(catalog);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldBeNull();
            });
        }
    }
}
