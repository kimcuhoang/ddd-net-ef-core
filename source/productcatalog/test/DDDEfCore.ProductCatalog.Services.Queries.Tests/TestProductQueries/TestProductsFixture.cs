using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestProductQueries
{
    public class TestProductsFixture : SharedFixture
    {
        public Product Product { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.SeedingData(this.Product);
        }

        #endregion
    }
}
