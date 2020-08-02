using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCategoriesController
{
    [Collection(nameof(SharedFixture))]
    public class TestCategoryControllerFixture : SharedFixture
    {
        public Category Category { get; private set; }

        public string BaseUrl => $"api/categories";

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Category = Category.Create(this.AutoFixture.Create<string>());
            await this.SeedingData<Category,CategoryId>(this.Category);
        }

        #endregion
    }
}
