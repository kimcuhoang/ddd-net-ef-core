using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalog
{
    public class TestCatalogModification
    {
        private readonly IFixture _fixture;

        public TestCatalogModification() => this._fixture = new Fixture();

        #region For Catalogs

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

        #endregion

        #region Catalog with CatalogCategory

        [Fact(DisplayName = "Catalog Create Chain of CatalogCategories Successfully")]
        public void Catalog_Create_Chain_Of_CatalogCategories_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryIdLv1 = IdentityFactory.Create<CategoryId>();
            var categoryIdLv2 = IdentityFactory.Create<CategoryId>();
            var categoryIdLv3 = IdentityFactory.Create<CategoryId>();

            var catalogCategoryLv1 = 
                catalog.AddCategory(categoryIdLv1, this._fixture.Create<string>());

            var catalogCategoryLv2 =
                catalog.AddCategory(categoryIdLv2, this._fixture.Create<string>(), catalogCategoryLv1);

            var catalogCategoryLv3 =
                catalog.AddCategory(categoryIdLv3, this._fixture.Create<string>(), catalogCategoryLv2);

            var catalogCategories = new List<CatalogCategory>
            {
                catalogCategoryLv1,
                catalogCategoryLv2,
                catalogCategoryLv3
            };

            catalog
                .Categories
                .Except(catalogCategories)
                .Any()
                .ShouldBeFalse();


            var roots = catalog.FindCatalogCategoryRoots();
            roots.ShouldHaveSingleItem();

            var descendantsOfLv1 = catalog.GetDescendantsOfCatalogCategory(catalogCategoryLv1);
            descendantsOfLv1
                .Except(catalogCategories)
                .Any()
                .ShouldBeFalse();

            var descendantsOfLv2 = catalog.GetDescendantsOfCatalogCategory(catalogCategoryLv2);
            descendantsOfLv2
                .Except(catalogCategories.Where(x => x != catalogCategoryLv1))
                .Any()
                .ShouldBeFalse();

            var descendantsOfLv3 = catalog.GetDescendantsOfCatalogCategory(catalogCategoryLv3);
            descendantsOfLv3
                .Except(catalogCategories.Where(x => x != catalogCategoryLv1 && x != catalogCategoryLv2))
                .Any()
                .ShouldBeFalse();
        }

        [Fact(DisplayName = "Catalog Remove CatalogCategory Within Descendants Successfully")]
        public void Catalog_Remove_CatalogCategory_Within_Descendants_Successfully()
        {
            var catalog = Catalog.Create(this._fixture.Create<string>());

            var categoryIdLv1 = IdentityFactory.Create<CategoryId>();
            var categoryIdLv2 = IdentityFactory.Create<CategoryId>();
            var categoryIdLv3 = IdentityFactory.Create<CategoryId>();

            var catalogCategoryLv1 =
                catalog.AddCategory(categoryIdLv1, this._fixture.Create<string>());

            var catalogCategoryLv2 =
                catalog.AddCategory(categoryIdLv2, this._fixture.Create<string>(), catalogCategoryLv1);

            var catalogCategoryLv3 =
                catalog.AddCategory(categoryIdLv3, this._fixture.Create<string>(), catalogCategoryLv2);

            catalog.RemoveCatalogCategoryWithDescendants(catalogCategoryLv1);
            catalog.Categories.ShouldBeEmpty();
        }

        #endregion
    }
}
