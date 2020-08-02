using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCatalog
{
    public class TestCatalogCreation
    {
        private readonly IFixture _fixture;

        public TestCatalogCreation() => this._fixture = new Fixture();

        [Theory(DisplayName = "Create Catalog with Display Name Successfully")]
        [AutoData]
        public void CreateCatalog_WithDisplayName_Successfully(string catalogName)
        {
            var catalog = Catalog.Create(catalogName);
            catalog.ShouldNotBeNull();
            catalog.DisplayName.ShouldBe(catalogName);
            catalog.Id.ShouldNotBeNull();
        }

        [Fact(DisplayName = "Create Catalog Without Display Name Should Throw Exception")]
        public void CreateCatalog_WithoutDisplayName_ShouldThrowException()
        {
            Should.Throw<DomainException>(() => Catalog.Create(string.Empty));
        }
        
    }
}
