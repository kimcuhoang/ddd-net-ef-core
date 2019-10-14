using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
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

        [Fact(DisplayName = "Add Product Successfully")]
        public void Add_Product_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = IdentityFactory.Create<ProductId>();
            var catalogProduct = catalogCategory.AddProduct(productId, this._fixture.Create<string>());

            catalogCategory.Products.ShouldContain(catalogProduct);
            catalogProduct.ShouldNotBeNull();
            catalogProduct.CatalogCategory.ShouldBe(catalogCategory);
        }

        [Fact(DisplayName = "Add Duplication of Product Should Throw Exception")]
        public void Add_Duplication_Of_Product_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = IdentityFactory.Create<ProductId>();
            var catalogProduct = catalogCategory.AddProduct(productId, this._fixture.Create<string>());

            Should.Throw<DomainException>(() => catalogCategory.AddProduct(productId, this._fixture.Create<string>()));
        }

        [Fact(DisplayName = "Add Null Of ProductId Should Throw Exception")]
        public void Add_Null_Of_ProductId_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            Should.Throw<DomainException>(() => catalogCategory.AddProduct(null, this._fixture.Create<string>()));
        }

        [Fact(DisplayName = "Add Product without DisplayName Should Throw Exception")]
        public void Add_Product_Without_DisplayName_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = IdentityFactory.Create<ProductId>();

            Should.Throw<DomainException>(() => catalogCategory.AddProduct(productId, string.Empty));
        }

        [Fact(DisplayName = "Remove CatalogProduct from CatalogCategory Successfully")]
        public void Remove_CatalogProduct_From_CatalogCategory_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

            var productId = IdentityFactory.Create<ProductId>();
            var catalogProduct = catalogCategory.AddProduct(productId, this._fixture.Create<string>());

            catalogCategory.RemoveCatalogProduct(catalogProduct);
            catalogCategory.Products.ShouldBeEmpty();
        }
    }
}
