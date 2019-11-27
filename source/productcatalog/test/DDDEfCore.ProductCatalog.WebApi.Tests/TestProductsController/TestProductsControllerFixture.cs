using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestProductsController
{
    [Collection(nameof(SharedFixture))]
    public class TestProductsControllerFixture : SharedFixture
    {
        public string BaseUrl => $"api/products";
        public Product Product { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Product = Product.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Product);
        }

        #endregion
    }
}
