using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries
{
    public class TestGetCategoryFixture : SharedFixture
    {
        public List<Category> Categories { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Categories = new List<Category>();

            var numberOfCategories = GenFu.GenFu.Random.Next(10);
            Enumerable.Range(0, numberOfCategories).ToList().ForEach(i =>
            {
                var category = Category.Create(this.Fixture.Create<string>());
                this.Categories.Add(category);
            });

            await this.SeedingData(this.Categories.ToArray());
        }

        #endregion
    }
}
