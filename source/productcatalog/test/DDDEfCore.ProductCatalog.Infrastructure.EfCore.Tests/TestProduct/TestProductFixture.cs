using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestProduct
{
    public class TestProductFixture : SharedFixture
    {
        public Product Product { get; private set; }
        public Product ProductToRemove { get; private set; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.ProductToRemove = Product.Create(this.Fixture.Create<string>());
            await this.SeedingData<Product, ProductId>(this.ProductToRemove);

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.RepositoryExecute<Product, ProductId>(async repository =>
            {
                await repository.AddAsync(this.Product);
                
            });
        }
        public async Task DoAssert(ProductId productId, Action<Product> assertFor)
        {
            await this.RepositoryExecute<Product, ProductId>(async repository =>
            {
                var product = await repository
                    .FindOneAsync(x => x.Id == productId);

                assertFor(product);
            });
        }
    }
}
