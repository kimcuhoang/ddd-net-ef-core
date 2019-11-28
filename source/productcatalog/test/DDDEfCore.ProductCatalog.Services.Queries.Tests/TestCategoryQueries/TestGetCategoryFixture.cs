using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries
{
    public class TestGetCategoryFixture : SharedFixture
    {
        public Category Category { get; private set; }
        public Catalog Catalog { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Category = Category.Create(this.Fixture.Create<string>());
            await this.SeedingData(this.Category);

            this.Catalog = Catalog.Create(this.Fixture.Create<string>());
            this.CatalogCategory = this.Catalog.AddCategory(this.Category.CategoryId, this.Category.DisplayName);
            await this.SeedingData(this.Catalog);
        }

        #endregion
    }
}
