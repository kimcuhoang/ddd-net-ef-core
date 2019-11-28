using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCategoryCommands
{
    public class TestRemoveCatalogProductCommand : UnitTestBase<Catalog>, IAsyncLifetime
    {
        private Mock<DbContext> _mockDbContext;
        private IRequestHandler<RemoveCatalogProductCommand> _requestHandler;
        private RemoveCatalogProductCommandValidator _validator;

        private Catalog _catalog;
        private Category _category;
        private Product _product;
        private CatalogCategory _catalogCategory;
        private CatalogProduct _catalogProduct;

        #region Implementation of IAsyncLifetime

        public Task InitializeAsync()
        {
            this._mockDbContext = new Mock<DbContext>();
            var catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);

            this.MockRepositoryFactory.Setup(x => x.CreateRepository<Catalog>())
                .Returns(catalogRepository);

            this._catalog = Catalog.Create(this.Fixture.Create<string>());
            this._category = Category.Create(this.Fixture.Create<string>());
            this._product = Product.Create(this.Fixture.Create<string>());

            this._catalogCategory = this._catalog.AddCategory(this._category.CategoryId, this._category.DisplayName);
            this._catalogProduct =
                this._catalogCategory.CreateCatalogProduct(this._product.ProductId, this._product.Name);

            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .Returns((new List<Catalog> {this._catalog}).AsQueryable().BuildMockDbSet().Object);

            this._validator = new RemoveCatalogProductCommandValidator(this.MockRepositoryFactory.Object);
            this._requestHandler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);
            return Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        [Fact(DisplayName = "Remove CatalogProduct Successfully")]
        public async Task Remove_CatalogProduct_Successfully()
        {
            var command = new RemoveCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                CatalogProductId = this._catalogProduct.CatalogProductId
            };

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
        }

        [Fact(DisplayName = "Invalid Command Should Throw ValidationException")]
        public async Task Invalid_Command_ShouldThrow_ValidationException()
        {
            var command = new RemoveCatalogProductCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty,
                CatalogProductId = (CatalogProductId)Guid.Empty
            };

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._requestHandler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Validation: Empty Command Should Be Invalid")]
        public void Empty_Command_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty,
                CatalogProductId = (CatalogProductId)Guid.Empty
            };
            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
        }

        [Fact(DisplayName = "Validation: Catalog Not Found Should Be Invalid")]
        public void Catalog_NotFound_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand
            {
                CatalogId = IdentityFactory.Create<CatalogId>(),
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>(),
                CatalogProductId = IdentityFactory.Create<CatalogProductId>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogProductId);
        }

        [Fact(DisplayName = "Validation: CatalogCategory Not Found Should Be Invalid")]
        public void CatalogCategory_NotFound_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>(),
                CatalogProductId = IdentityFactory.Create<CatalogProductId>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogProductId);
        }

        [Fact(DisplayName = "Validation: CatalogProduct Not Found Should Be Invalid")]
        public void CatalogProduct_NotFound_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                CatalogProductId = IdentityFactory.Create<CatalogProductId>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        }
    }
}
