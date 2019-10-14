using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public CatalogCategory CatalogCategoryLv1 { get; private set; }
        public CatalogCategory CatalogCategoryLv2 { get; private set; }
        public CatalogCategory CatalogCategoryLv3 { get; private set; }

        public IEnumerable<CatalogCategory> CatalogCategories => new List<CatalogCategory>
        {
            this.CatalogCategoryLv1,
            this.CatalogCategoryLv2,
            this.CatalogCategoryLv3
        };

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

            this.CatalogCategoryLv1 =
                this.Catalog.AddCategory(this.CategoryLv1.CategoryId, this.Fixture.Create<string>());
            this.CatalogCategoryLv2 =
                this.Catalog.AddCategory(this.CategoryLv2.CategoryId, this.Fixture.Create<string>(), this.CatalogCategoryLv1);
            this.CatalogCategoryLv3 =
                this.Catalog.AddCategory(this.CategoryLv3.CategoryId, this.Fixture.Create<string>(), this.CatalogCategoryLv2);

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
