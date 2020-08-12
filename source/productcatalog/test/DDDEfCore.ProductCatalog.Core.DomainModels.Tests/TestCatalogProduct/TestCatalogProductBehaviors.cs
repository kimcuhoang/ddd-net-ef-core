using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalogProduct
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
