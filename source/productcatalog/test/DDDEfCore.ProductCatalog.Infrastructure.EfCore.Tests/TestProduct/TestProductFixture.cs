using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestProduct;

public class TestProductFixture : DefaultTestFixture
{
    public TestProductFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Product Product { get; private set; }
    public Product ProductToRemove { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.ProductToRemove = Product.Create(this.Fixture.Create<string>());
        await this.SeedingData<Product, ProductId>(this.ProductToRemove);

        this.Product = Product.Create(this.Fixture.Create<string>());

        await this.ExecuteTransactionDbContextAsync(async _ =>
        {
            _.Add(this.Product);
            await _.SaveChangesAsync();
        });
    }
    public async Task DoAssert(ProductId productId, Action<Product> assertFor)
    {
        await this.ExecuteDbContextAsync(async _ =>
        {
            var product = await _.Set<Product>().FirstOrDefaultAsync(_ => _.Id == productId);
            assertFor(product);
        });
    }
}
