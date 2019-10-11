using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestProduct
{
    [Collection(nameof(SharedFixture))]
    public class TestProductFixture : BaseTestFixture<Product>
    {
        public TestProductFixture(SharedFixture sharedFixture) : base(sharedFixture) { }
        
        public Product Product { get; private set; }


        #region Overrides of BaseTestFixture<Category>

        public override async Task InitData()
        {
            this.Product = Product.Create(this.Fixture.Create<string>());

            await this.RepositoryExecute(async repository => { await repository.AddAsync(this.Product); });
        }

        public override async Task DoAssert(Action<Product> assertFor)
        {
            await this.RepositoryExecute(async repository =>
            {
                var product = await repository.FindOneAsync(x => x.ProductId == this.Product.ProductId);

                assertFor(product);
            });
        }

        #endregion
    }
}
