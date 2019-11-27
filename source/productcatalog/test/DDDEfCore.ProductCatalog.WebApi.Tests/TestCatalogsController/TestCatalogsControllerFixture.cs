using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogsControllerFixture : SharedFixture
    {
        public string BaseUrl => @"api/catalogs";
        public Catalog Catalog { get; private set; }
        public Category Category { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Catalog = Catalog.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Catalog);

            this.Category = Category.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Category);
        }

        #endregion
    }
}
