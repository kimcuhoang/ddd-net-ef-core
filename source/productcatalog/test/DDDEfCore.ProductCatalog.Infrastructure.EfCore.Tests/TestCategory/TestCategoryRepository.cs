using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory
{
    [Collection(nameof(SharedFixture))]
    public class TestCategoryRepository : IClassFixture<TestCategoryFixture>
    {
        private readonly TestCategoryFixture _testFixture;

        public TestCategoryRepository(TestCategoryFixture testFixture)
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Fact(DisplayName = "Should Create Category Successfully")]
        public async Task ShouldCreateCategorySuccessfully()
        {
            await this._testFixture.DoAssert(category =>
            {
                category.ShouldNotBeNull();
                category.Equals(this._testFixture.Category).ShouldBeTrue();
            });
        }

        [Theory(DisplayName = "Should Update Category Successfully")]
        [AutoData]
        public async Task ShouldUpdateCategorySuccessfully(string newCategoryName)
        {
            await this._testFixture.RepositoryExecute<Category, CategoryId>(async repository =>
            {
                var category =
                    await repository.FindOneAsync(x => x.Id == this._testFixture.Category.Id);

                category.ChangeDisplayName(newCategoryName);

                await repository.UpdateAsync(category);
            });

            await this._testFixture.DoAssert(category =>
            {
                category.ShouldNotBeNull();
                category.Equals(this._testFixture.Category).ShouldBeTrue();
                category.DisplayName.ShouldBe(newCategoryName);
            });
        }

        [Fact(DisplayName = "Should Remove Category Successfully")]
        public async Task ShouldRemoveCategorySuccessfully()
        {
            await this._testFixture.RepositoryExecute<Category, CategoryId>(async repository =>
            {
                var category = await repository.FindOneAsync(x => x.Id == this._testFixture.Category.Id);
                await repository.RemoveAsync(category);
            });

            await this._testFixture.DoAssert(category =>
            {
                category.ShouldBeNull();
            });
        }
    }
}
