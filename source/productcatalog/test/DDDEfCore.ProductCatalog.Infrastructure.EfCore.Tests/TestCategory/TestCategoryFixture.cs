using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory
{
    [Collection(nameof(SharedFixture))]
    public class TestCategoryFixture : BaseTestFixture<Category>
    {
        public TestCategoryFixture(SharedFixture sharedFixture) : base(sharedFixture) { }
        
        public Category Category { get; private set; }


        #region Overrides of BaseTestFixture<Category>

        public override async Task InitData()
        {
            this.Category = Category.Create(this.Fixture.Create<string>());

            await this.RepositoryExecute(async repository => { await repository.AddAsync(this.Category); });
        }

        public override async Task DoAssert(Action<Category> assertFor)
        {
            await this.RepositoryExecute(async repository =>
            {
                var category = await repository.FindOneAsync(x => x.CategoryId == this.Category.CategoryId);

                assertFor(category);
            });
        }

        #endregion
    }
}
