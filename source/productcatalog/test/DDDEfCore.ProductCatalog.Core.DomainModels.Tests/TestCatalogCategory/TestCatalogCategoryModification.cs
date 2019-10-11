using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
