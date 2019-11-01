using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
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
            await this._testFixture.DoAssert(this._testFixture.Product.ProductId, product =>
            {
                product.ShouldNotBeNull();
                product.Equals(this._testFixture.Product).ShouldBeTrue();
            });
        }

        [Theory(DisplayName = "Should Update Product Successfully")]
        [AutoData]
        public async Task ShouldUpdateProductSuccessfully(string newProductName)
        {
            await this._testFixture.RepositoryExecute<Product>(async repository =>
            {
                var product =
                    await repository.FindOneAsync(x => x.ProductId == this._testFixture.Product.ProductId);

                product.ChangeName(newProductName);

                await repository.UpdateAsync(product);
            });

            await this._testFixture.DoAssert(this._testFixture.Product.ProductId, product =>
            {
                product.ShouldNotBeNull();
                product.Equals(this._testFixture.Product).ShouldBeTrue();
                product.Name.ShouldBe(newProductName);
            });
        }

        [Fact(DisplayName = "Should Remove Product Successfully")]
        public async Task ShouldRemoveProductSuccessfully()
        {
            var product = this._testFixture.ProductToRemove;

            await this._testFixture.RepositoryExecute<Product>(async repository =>
            {
                var productToRemove = await repository
                    .FindOneAsync(x => x.ProductId == product.ProductId);

                await repository.RemoveAsync(productToRemove);
            });

            await this._testFixture.DoAssert(product.ProductId,
                productAssert => { productAssert.ShouldBeNull(); });
        }
    }
}
