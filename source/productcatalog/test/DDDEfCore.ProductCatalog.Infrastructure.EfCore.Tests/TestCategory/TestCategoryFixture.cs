using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory
{
    public class TestCategoryFixture : SharedFixture
    {
        public Category Category { get; private set; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Category = Category.Create(this.Fixture.Create<string>());

            await this.RepositoryExecute<Category>(async repository =>
            {
                await repository.AddAsync(this.Category);
            });
        }

        public async Task DoAssert(Action<Category> assertFor)
        {
            await this.RepositoryExecute<Category>(async repository =>
            {
                var category = await repository
                    .FindOneAsync(x => x.CategoryId == this.Category.CategoryId);

                assertFor(category);
            });
        }

    }
}
