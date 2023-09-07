using AutoFixture;
using AutoFixture.Xunit2;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Exceptions;
using DDD.ProductCatalog.Core.Products;
using Shouldly;
using Xunit;

namespace DDD.ProductCatalog.Core.Tests.TestCatalogProduct
{
    public class TestCatalogProductBehaviors
    {
        private readonly IFixture _fixture;

        public TestCatalogProductBehaviors()
            => this._fixture = new Fixture();

        private CatalogProduct InitCatalogProduct(string creationName)
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = ProductId.New;
            var catalogProduct = catalogCategory
                .CreateCatalogProduct(productId, creationName);

            return catalogProduct;
        }

        [Theory(DisplayName = "CatalogProduct Change DisplayName Successfully")]
        [AutoData]
        public void CatalogProduct_Change_DisplayName_Successfully(string creationName, string changeToName)
        {
            var catalogProduct = this.InitCatalogProduct(creationName);

            catalogProduct.ChangeDisplayName(changeToName);

            catalogProduct.DisplayName.ShouldBe(changeToName);
        }

        [Theory(DisplayName = "CatalogProduct Change DisplayName To Empty Should Throw Exception")]
        [AutoData]
        public void CatalogProduct_Change_DisplayName_To_Empty_ShouldThrowException(string creationName)
        {
            var catalogProduct = this.InitCatalogProduct(creationName);

            Should.Throw<DomainException>(() => catalogProduct.ChangeDisplayName(string.Empty));
        }
    }
}
