using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Shouldly;
using System;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalogCategory
{
    public class TestCatalogCategoryCreation
    {
        [Fact(DisplayName = "Create CatalogCategory Successfully")]
        public void Create_CatalogCategory_Successfully()
        {
            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogId = IdentityFactory.Create<CatalogId>();

            var catalogCategory = CatalogCategory.Create(catalogId, categoryId);

            catalogCategory.ShouldNotBeNull();
            catalogCategory.CatalogCategoryId.ShouldNotBeNull();
            catalogCategory.CatalogId.ShouldBe(catalogId);
            catalogCategory.CategoryId.ShouldBe(categoryId);
        }

        [Fact(DisplayName = "Create Catalog Category With Null of CatalogId Should Throw Exception")]
        public void Create_CatalogCategory_With_NullOf_CatalogId_Should_Throw_Exception()
        {
            var categoryId = IdentityFactory.Create<CategoryId>();
            Should.Throw<ArgumentNullException>(() => CatalogCategory.Create(null, categoryId));
        }

        [Fact(DisplayName = "Create Catalog Category With Null of CategoryId Should Throw Exception")]
        public void Create_CatalogCategory_With_NullOf_CategoryId_Should_Throw_Exception()
        {
            var catalogId = IdentityFactory.Create<CatalogId>();
            Should.Throw<ArgumentNullException>(() => CatalogCategory.Create(catalogId, null));
        }
    }
}
