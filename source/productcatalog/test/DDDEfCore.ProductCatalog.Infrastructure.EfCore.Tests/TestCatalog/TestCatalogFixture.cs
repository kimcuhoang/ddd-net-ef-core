using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogFixture : BaseTestFixture<Catalog>, IAsyncLifetime
    {
        public TestCatalogFixture(SharedFixture sharedFixture) : base(sharedFixture) { }

        public Catalog Catalog { get; private set; }
        public Category CategoryLv1 { get; private set; }
        public Category CategoryLv2 { get; private set; }
        public Category CategoryLv3 { get; private set; }

        #region Implementation of IAsyncLifetime

        public async Task InitializeAsync()
        {
            this.CategoryLv1 = Category.Create(this.Fixture.Create<string>());
            this.CategoryLv2 = Category.Create(this.Fixture.Create<string>());
            this.CategoryLv3 = Category.Create(this.Fixture.Create<string>());

            await this.SharedFixture.SeedingData(this.CategoryLv1,
                                                        this.CategoryLv2,
                                                        this.CategoryLv3);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        #region Overrides of BaseTestFixture<Catalog>

        public override async Task InitData()
        {
            this.Catalog = Catalog.Create(this.Fixture.Create<string>());

            var subCategoryLv1 = this.Catalog.AddCategoryRoot(this.CategoryLv1.CategoryId);
            subCategoryLv1.WithDisplayName("Lv1");

            var subCategoryLv2 = subCategoryLv1.AddSubCategory(this.CategoryLv2.CategoryId);
            subCategoryLv2.WithDisplayName("Lv2");

            var subCategoryLv3 = subCategoryLv2.AddSubCategory(this.CategoryLv3.CategoryId);
            subCategoryLv3.WithDisplayName("Lv3");

            await this.RepositoryExecute(async repository =>
            {
                await repository.AddAsync(this.Catalog);
            });
        }

        public override async Task DoAssert(Action<Catalog> assertFor)
        {
            await this.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(y => y.Categories));

                assertFor(catalog);
            });
        }

        #endregion
    }
}
