using AutoFixture;
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
            var command = new RemoveCatalogProductCommand(this._catalog.CatalogId.Id,
                                            this._catalogCategory.CatalogCategoryId.Id,
                                            this._catalogProduct.CatalogProductId.Id);

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
        }

        [Fact(DisplayName = "Invalid Command Should Throw ValidationException")]
        public async Task Invalid_Command_ShouldThrow_ValidationException()
        {
            var command = new RemoveCatalogProductCommand(Guid.Empty, Guid.Empty, Guid.Empty);

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._requestHandler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Validation: Empty Command Should Be Invalid")]
        public void Empty_Command_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand(Guid.Empty, Guid.Empty, Guid.Empty);
            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
        }

        [Fact(DisplayName = "Validation: Catalog Not Found Should Be Invalid")]
        public void Catalog_NotFound_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand(Guid.NewGuid(), 
                                            Guid.NewGuid(), 
                                            Guid.NewGuid());

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogProductId);
        }

        [Fact(DisplayName = "Validation: CatalogCategory Not Found Should Be Invalid")]
        public void CatalogCategory_NotFound_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand(this._catalog.CatalogId.Id,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogProductId);
        }

        [Fact(DisplayName = "Validation: CatalogProduct Not Found Should Be Invalid")]
        public void CatalogProduct_NotFound_ShouldBe_Invalid()
        {
            var command = new RemoveCatalogProductCommand(this._catalog.CatalogId.Id,
                this._catalogCategory.CatalogCategoryId.Id,
                Guid.NewGuid());

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        }
    }
}
