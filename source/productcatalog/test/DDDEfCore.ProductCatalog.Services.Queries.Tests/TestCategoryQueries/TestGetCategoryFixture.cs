using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries
{
    public class TestGetCategoryFixture : SharedFixture
    {
        public List<Category> Categories { get; private set; }

        public Catalog Catalog { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await this.InitCategories();
            await this.InitCatalogsWithCategoriesAssociation();
        }

        #endregion

        private async Task InitCategories()
        {
            this.Categories = new List<Category>();

            var numberOfCategories = GenFu.GenFu.Random.Next(10);
            Enumerable.Range(0, numberOfCategories).ToList().ForEach(i =>
            {
                var category = Category.Create(this.Fixture.Create<string>());
                this.Categories.Add(category);
            });

            await this.SeedingData(this.Categories.ToArray());
        }

        private async Task InitCatalogsWithCategoriesAssociation()
        {
            this.Catalog = Catalog.Create(this.Fixture.Create<string>());
            this.Categories.ForEach(category => this.Catalog.AddCategory(category.CategoryId, category.DisplayName));

            await this.SeedingData(this.Catalog);
        }
    }
}
