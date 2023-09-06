using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestProduct;

public class TestProductRepository : TestBase<TestProductFixture>
{
    public TestProductRepository(ITestOutputHelper testOutput, TestProductFixture fixture) : base(testOutput, fixture)
    {
    }

    [Fact(DisplayName = "Should Create Product Successfully")]
    public async Task ShouldCreateProductSuccessfully()
    {
        await this._fixture.DoAssert(this._fixture.Product.Id, product =>
        {
            product.ShouldNotBeNull();
            product.Equals(this._fixture.Product).ShouldBeTrue();
        });
    }

    [Theory(DisplayName = "Should Update Product Successfully")]
    [AutoData]
    public async Task ShouldUpdateProductSuccessfully(string newProductName)
    {
        await this._fixture.RepositoryExecute<Product, ProductId>(async repository =>
        {
            var product = await repository.FindOneAsync(x => x == this._fixture.Product);
            product.ShouldNotBeNull();
            product.ChangeName(newProductName);
        });

        await this._fixture.DoAssert(this._fixture.Product.Id, product =>
        {
            product.ShouldNotBeNull();
            product.Equals(this._fixture.Product).ShouldBeTrue();
            product.Name.ShouldBe(newProductName);
        });
    }

    [Fact(DisplayName = "Should Remove Product Successfully")]
    public async Task ShouldRemoveProductSuccessfully()
    {
        var product = this._fixture.ProductToRemove;

        await this._fixture.RepositoryExecute<Product, ProductId>(async repository =>
        {
            var productToRemove = await repository
                .FindOneAsync(x => x.Id == product.Id);

            repository.Remove(productToRemove);
        });

        await this._fixture.DoAssert(product.Id, productAssert => { productAssert.ShouldBeNull(); });
    }
}
