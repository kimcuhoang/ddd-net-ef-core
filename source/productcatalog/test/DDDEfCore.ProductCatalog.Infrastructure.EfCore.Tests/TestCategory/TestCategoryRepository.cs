using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory
{
    [Collection(nameof(SharedFixture))]
    public class TestCategoryRepository : IClassFixture<SharedRepositoryFixture<Category>>
    {
        private readonly SharedRepositoryFixture<Category> _testFixture;

        public TestCategoryRepository(SharedRepositoryFixture<Category> testFixture)
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Theory(DisplayName = "Should Create Category Successfully")]
        [MemberData(nameof(CategoryDataTest.NewCategory), MemberType = typeof(CategoryDataTest))]
        public async Task ShouldCreateCategorySuccessfully(Category category)
        {
            await this._testFixture.RepositoryExecute(async repository => { await repository.AddAsync(category); });

            var assertCategory = await this._testFixture.RepositoryQuery(async repository =>
            {
                return await repository.FindOneAsync(x => x.CategoryId == category.CategoryId);
            });

            assertCategory.ShouldNotBeNull();
            assertCategory.Equals(category).ShouldBeTrue();
        }

        [Theory(DisplayName = "Should Update Category Successfully")]
        [MemberData(nameof(CategoryDataTest.UpdateCategory), MemberType = typeof(CategoryDataTest))]
        public async Task ShouldUpdateCategorySuccessfully(Category category, string aNewName)
        {
            await this._testFixture.RepositoryExecute(async repository => { await repository.AddAsync(category); });

            category = await this._testFixture.RepositoryQuery(async repository =>
            {
                return await repository.FindOneAsync(x => x.CategoryId == category.CategoryId);
            });

            category.ChangeDisplayName(aNewName);
            await this._testFixture.RepositoryExecute(async repository => { await repository.UpdateAsync(category); });

            var assertCategory = await this._testFixture.RepositoryQuery(async repository =>
            {
                return await repository.FindOneAsync(x => x.CategoryId == category.CategoryId);
            });

            assertCategory.ShouldNotBeNull();
            assertCategory.Equals(category).ShouldBeTrue();
            assertCategory.DisplayName.ShouldBe(aNewName);
        }

        [Theory(DisplayName = "Should Remove Category Successfully")]
        [MemberData(nameof(CategoryDataTest.NewCategory), MemberType = typeof(CategoryDataTest))]
        public async Task ShouldRemoveCategorySuccessfully(Category category)
        {
            await this._testFixture.RepositoryExecute(async repository => { await repository.AddAsync(category); });

            category = await this._testFixture.RepositoryQuery(async repository =>
            {
                return await repository.FindOneAsync(x => x.CategoryId == category.CategoryId);
            });

            await this._testFixture.RepositoryExecute(async repository => { await repository.RemoveAsync(category); });

            category = await this._testFixture.RepositoryQuery(async repository =>
            {
                return await repository.FindOneAsync(x => x.CategoryId == category.CategoryId);
            });
            category.ShouldBeNull();
        }
    }
}
