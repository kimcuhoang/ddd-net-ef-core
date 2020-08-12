using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalogCategory
{
    public class TestCatalogCategoryBehaviors
    {
        private readonly IFixture _fixture;

        public TestCatalogCategoryBehaviors()
            => this._fixture = new Fixture();

        #region Self Behaviors

        [Theory(DisplayName = "CatalogCategory Change DisplayName Successfully")]
        [AutoData]
        public void CatalogCategory_Change_DisplayName_Successfully(string creationName, string changeToName)
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog
                                    .AddCategory(categoryId, creationName)
                                    .ChangeDisplayName(changeToName);

            catalogCategory.DisplayName.ShouldBe(changeToName);
        }

        [Theory(DisplayName = "CatalogCategory Change DisplayName To Empty Should Throw Exception")]
        [AutoData]
        public void CatalogCategory_Change_DisplayName_ToEmpty_ShouldThrowException(string creationName)
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, creationName);

            Should.Throw<DomainException>(() => catalogCategory.ChangeDisplayName(string.Empty));
        }

        #endregion

        #region Behaviors With CatalogProduct

        [Fact(DisplayName = "Create CatalogProduct Successfully")]
        public void Create_CatalogProduct_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = ProductId.New;
            var catalogProduct = catalogCategory.CreateCatalogProduct(productId, this._fixture.Create<string>());

            catalogCategory.Products.ShouldContain(catalogProduct);
            catalogProduct.ShouldNotBeNull();
            catalogProduct.CatalogCategory.ShouldBe(catalogCategory);
        }

        [Fact(DisplayName = "Create CatalogProduct With Duplication of ProductId Should Throw Exception")]
        public void Create_CatalogProduct_With_Duplication_Of_ProductId_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = ProductId.New;
            var catalogProduct = catalogCategory.CreateCatalogProduct(productId, this._fixture.Create<string>());

            Should.Throw<DomainException>(() => catalogCategory.CreateCatalogProduct(productId, this._fixture.Create<string>()));
        }

        [Fact(DisplayName = "Create CatalogProduct With Null Of ProductId Should Throw Exception")]
        public void Create_CatalogProduct_With_Null_Of_ProductId_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            Should.Throw<DomainException>(() => catalogCategory.CreateCatalogProduct(null, this._fixture.Create<string>()));
        }

        [Fact(DisplayName = "Create CatalogProduct without DisplayName Should Throw Exception")]
        public void Create_CatalogProduct_Without_DisplayName_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = ProductId.New;

            Should.Throw<DomainException>(() => catalogCategory.CreateCatalogProduct(productId, string.Empty));
        }

        [Fact(DisplayName = "Remove CatalogProduct Successfully")]
        public void Remove_CatalogProduct_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = ProductId.New;
            var catalogProduct = catalogCategory.CreateCatalogProduct(productId, this._fixture.Create<string>());

            catalogCategory.RemoveCatalogProduct(catalogProduct);
            catalogCategory.Products.ShouldBeEmpty();
        }

        [Fact(DisplayName = "Remove Null Of CatalogProduct Should Throw Exception")]
        public void Remove_Null_Of_CatalogProduct_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = CategoryId.New;
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var catalogProduct = catalogCategory.Products.FirstOrDefault();

            Should.Throw<DomainException>(() => catalogCategory.RemoveCatalogProduct(catalogProduct));
        }

        #endregion
    }
}
