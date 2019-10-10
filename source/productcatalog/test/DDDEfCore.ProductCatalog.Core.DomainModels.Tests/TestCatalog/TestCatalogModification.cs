using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalog
{
    public class TestCatalogModification
    {
        private readonly IFixture _fixture;

        public TestCatalogModification() => this._fixture = new Fixture();

        [Theory(DisplayName = "Modify Catalog By Changing Display Name Successfully")]
        [AutoData]
        public void ModifyCatalog_By_ChangingDisplayName_Successfully(string originName, string changeToName)
        {
            var catalog = Catalog.Create(originName);
            catalog.ChangeDisplayName(changeToName);

            catalog.DisplayName.ShouldBe(changeToName);
        }

        [Fact(DisplayName = "Modify Catalog By Changing Display Name with Empty Should Throw Exception")]
        public void ModifyCatalog_By_ChangingDisplayName_WithEmpty_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            Should.Throw<DomainException>(() => catalog.ChangeDisplayName(string.Empty));
        }

        [Fact(DisplayName = "Modify Catalog By Adding Sub Categories Successfully")]
        public void ModifyCatalog_By_AddingSubCategories_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();

            var catalogCategory = catalog.AddCategoryRoot(categoryId);

            catalog.ShouldNotBeNull();
            catalog.Categories.ShouldHaveSingleItem();
            catalog.Categories.ShouldBeAssignableTo<IEnumerable<CatalogCategory>>();

            catalogCategory.ShouldNotBeNull();
            catalogCategory.ShouldBeAssignableTo<CatalogCategory>();
            catalogCategory.CatalogCategoryId.ShouldNotBeNull();
        }

        [Fact(DisplayName = "Modify Catalog By Adding Duplicate Categories Should Throw Exception")]
        public void ModifyCatalog_By_AddingDuplicateCategories_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryId = IdentityFactory.Create<CategoryId>();

            catalog.AddCategoryRoot(categoryId);

            Should.Throw<DomainException>(() => catalog.AddCategoryRoot(categoryId));
        }

        [Fact(DisplayName = "Modify Catalog By Adding Null of Category Should Throw Exception")]
        public void ModifyCatalog_By_Adding_NullOfCategory_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());
            Should.Throw<DomainException>(() => catalog.AddCategoryRoot(null));
        }

        [Fact(DisplayName = "Modify Catalog By Removing Null of Category Should Throw Exception")]
        public void ModifyCatalog_By_Removing_NullOfCategory_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());
            Should.Throw<DomainException>(() => catalog.RemoveCategoryWithDescendants(null));
        }

        [Fact(DisplayName = "Modify Catalog By Removing Undefined Category Should Throw Exception")]
        public void ModifyCatalog_By_Removing_UndefinedCategory_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());
            var categoryId = IdentityFactory.Create<CategoryId>();
            Should.Throw<DomainException>(() => catalog.RemoveCategoryWithDescendants(categoryId));
        }

        [Fact(DisplayName = "Modify Catalog By Removing Sub Category Successfully")]
        public void ModifyCatalog_By_Removing_SubCategory_ShouldThrowException()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());
            var categoryId = IdentityFactory.Create<CategoryId>();
            catalog.AddCategoryRoot(categoryId);
            
            catalog.RemoveCategoryWithDescendants(categoryId);
            catalog.Categories.ShouldBeEmpty();
        }
    }
}
