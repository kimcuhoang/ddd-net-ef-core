using AutoFixture.Xunit2;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestProduct
{
    [Collection(nameof(SharedFixture))]
    public class TestProductRepository : IClassFixture<TestProductFixture>
    {
        private readonly TestProductFixture _testFixture;

        public TestProductRepository(TestProductFixture testFixture)
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Fact(DisplayName = "Should Create Product Successfully")]
        public async Task ShouldCreateProductSuccessfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.DoAssert(product =>
            {
                product.ShouldNotBeNull();
                product.Equals(this._testFixture.Product).ShouldBeTrue();
            });
        }

        [Theory(DisplayName = "Should Update Product Successfully")]
        [AutoData]
        public async Task ShouldUpdateProductSuccessfully(string newProductName)
        {
            await this._testFixture.InitData();
            
            await this._testFixture.RepositoryExecute(async repository =>
            {
                var product =
                    await repository.FindOneAsync(x => x.ProductId == this._testFixture.Product.ProductId);

                product.ChangeName(newProductName);

                await repository.UpdateAsync(product);
            });

            await this._testFixture.DoAssert(product =>
            {
                product.ShouldNotBeNull();
                product.Equals(this._testFixture.Product).ShouldBeTrue();
                product.Name.ShouldBe(newProductName);
            });
        }

        [Fact(DisplayName = "Should Remove Product Successfully")]
        public async Task ShouldRemoveProductSuccessfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.RepositoryExecute(async repository =>
            {
                var product = await repository.FindOneAsync(x => x.ProductId == this._testFixture.Product.ProductId);
                await repository.RemoveAsync(product);
            });

            await this._testFixture.DoAssert(category =>
            {
                category.ShouldBeNull();
            });
        }
    }
}
