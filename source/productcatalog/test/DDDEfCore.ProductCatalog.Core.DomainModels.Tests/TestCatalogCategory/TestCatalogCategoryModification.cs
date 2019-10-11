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
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalogCategory
{
    public class TestCatalogCategoryModification
    {
        private readonly IFixture _fixture;

        public TestCatalogCategoryModification()
            => this._fixture = new Fixture();

        [Theory(DisplayName = "Change Display Name Successfully")]
        [AutoData]
        public void ChangeDisplayNameSuccessfully(string aDisplayName)
        {
            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogId = IdentityFactory.Create<CatalogId>();
            
            var catalogCategory = CatalogCategory
                                    .Create(catalogId, categoryId)
                                    .WithDisplayName(aDisplayName);
            
            catalogCategory.DisplayName.ShouldBe(aDisplayName);
        }

        [Fact(DisplayName = "Change Display Name To Empty Should Throw Exception ")]
        public void ChangeDisplayNameToEmptyShouldThrowException()
        {
            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogId = IdentityFactory.Create<CatalogId>();

            var catalogCategory = CatalogCategory
                .Create(catalogId, categoryId);

            Should.Throw<DomainException>(() => catalogCategory.WithDisplayName(string.Empty));
        }

        [Fact(DisplayName = "Add Children of CatalogCategory as Tree Successfully")]
        public void Add_Children_As_Tree_Successfully()
        {
            var catalogId = IdentityFactory.Create<CatalogId>();

            var categoryLv1 = IdentityFactory.Create<CategoryId>();
            var categoryLv2 = IdentityFactory.Create<CategoryId>();
            var categoryLv3 = IdentityFactory.Create<CategoryId>();

            var subCategoryLv1 = CatalogCategory.Create(catalogId, categoryLv1).WithDisplayName("Lv1");
            var subCategoryLv2 = subCategoryLv1.AddSubCategory(categoryLv2).WithDisplayName("Lv2");
            var subCategoryLv3 = subCategoryLv2.AddSubCategory(categoryLv3).WithDisplayName("Lv3");

            subCategoryLv1.ShouldNotBeNull();
            subCategoryLv1.CatalogId.ShouldBe(catalogId);
            subCategoryLv1.CategoryId.ShouldBe(categoryLv1);

            subCategoryLv2.ShouldNotBeNull();
            subCategoryLv2.CatalogId.ShouldBe(catalogId);
            subCategoryLv2.CategoryId.ShouldBe(categoryLv2);

            subCategoryLv3.ShouldNotBeNull();
            subCategoryLv3.CatalogId.ShouldBe(catalogId);
            subCategoryLv3.CategoryId.ShouldBe(categoryLv3);


            subCategoryLv1.SubCategories.Count().ShouldBe(1);
            subCategoryLv1.Parent.ShouldBeNull();

            subCategoryLv2.SubCategories.Count().ShouldBe(1);
            subCategoryLv2.Parent.ShouldBe(subCategoryLv1);

            subCategoryLv3.SubCategories.ShouldBeEmpty();
            subCategoryLv3.Parent.ShouldBe(subCategoryLv2);
        }

        [Fact(DisplayName = "Get Children Recursively Correct")]
        public void Get_Children_Recursively_Correct()
        {
            var catalogId = IdentityFactory.Create<CatalogId>();

            var categoryLv1 = IdentityFactory.Create<CategoryId>();
            var categoryLv2 = IdentityFactory.Create<CategoryId>();
            var categoryLv3 = IdentityFactory.Create<CategoryId>();

            var subCategoryLv1 = CatalogCategory.Create(catalogId, categoryLv1).WithDisplayName("Lv1");
            var subCategoryLv2 = subCategoryLv1.AddSubCategory(categoryLv2).WithDisplayName("Lv2");
            var subCategoryLv3 = subCategoryLv2.AddSubCategory(categoryLv3).WithDisplayName("Lv3");

            var descendants = subCategoryLv1.GetDescendants();
            var expectedDescendants = new List<CatalogCategory> { subCategoryLv3, subCategoryLv2 };
            descendants.ShouldBe(expectedDescendants);
        }
    }
}
