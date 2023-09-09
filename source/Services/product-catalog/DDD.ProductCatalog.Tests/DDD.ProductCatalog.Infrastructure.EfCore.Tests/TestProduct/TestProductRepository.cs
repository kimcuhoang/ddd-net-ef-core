using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Tests.TestProduct;

public class TestProductRepository : TestEfCoreBase
{
    public TestProductRepository(TestEfCoreCollectionFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    [Fact(DisplayName = "Should Create Product Successfully")]
    public async Task ShouldCreateProductSuccessfully()
    {
        var product = Product.Create(this._fixture.Create<string>());

        await this.ExecuteTransactionDbContext(async _ =>
        {
            _.Add(product);
            await _.SaveChangesAsync();
        });

        await this.ExecuteDbContextAsync(async _ =>
        {
            var productSaved = await _.Set<Product>().FirstOrDefaultAsync(_ => _ == product);
            productSaved.ShouldNotBeNull();
            productSaved.Equals(product).ShouldBeTrue();
        });
    }
}
