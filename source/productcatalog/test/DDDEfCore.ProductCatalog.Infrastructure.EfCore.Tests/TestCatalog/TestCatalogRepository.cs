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

        [Fact]
        public async Task ShouldCreateCatalogWithSubCategoriesSuccessfully()
        {
            //await this._testFixture.RepositoryExecuteAsync(async repository => { await repository.AddAsync(this._testFixture.Catalog); });

            await this._testFixture.RepositoryExecuteAsync(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this._testFixture.Catalog.CatalogId,
                            x => x.Include(y => y.Categories));

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

        [Fact]
        public async Task ShouldDeleteCategoryLv1WithDescendantsSuccessfully()
        {
            //await this._testFixture.RepositoryExecuteAsync(async repository => { await repository.AddAsync(this._testFixture.Catalog); });

            await this._testFixture.RepositoryExecuteAsync(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this._testFixture.Catalog.CatalogId,
                        x => x.Include(y => y.Categories));

                catalog.RemoveCategoryWithDescendants(this._testFixture.CategoryLv1.CategoryId);

                await repository.UpdateAsync(catalog);
            });
        }
    }
}
