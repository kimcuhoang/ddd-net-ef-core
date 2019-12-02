using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCategoryCommands
{
    public class TestCreateCatalogProductCommand : UnitTestBase<Catalog>, IAsyncLifetime
    {
        private Mock<DbContext> _mockDbContext;
        private CreateCatalogProductCommandValidator _validator;
        private IRequestHandler<CreateCatalogProductCommand> _requestHandler;

        private Catalog _catalog;
        private Category _category;
        private Product _product;
        private CatalogCategory _catalogCategory;

        #region Implementation of IAsyncLifetime

        public Task InitializeAsync()
        {
            this._mockDbContext = new Mock<DbContext>();
            var catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            var productRepository = new DefaultRepositoryAsync<Product>(this._mockDbContext.Object);

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>()).Returns(catalogRepository);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Product>()).Returns(productRepository);

            this._catalog = Catalog.Create(this.Fixture.Create<string>());
            this._category = Category.Create(this.Fixture.Create<string>());
            this._product = Product.Create(this.Fixture.Create<string>());
            this._catalogCategory = this._catalog.AddCategory(this._category.CategoryId, this._category.DisplayName);

            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .ReturnsDbSet(new List<Catalog> {this._catalog});
            this._mockDbContext
                .Setup(x => x.Set<Product>())
                .ReturnsDbSet(new List<Product> {this._product});

            this._validator = new CreateCatalogProductCommandValidator(this.MockRepositoryFactory.Object);
            this._requestHandler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            return Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        [Fact(DisplayName = "Create CatalogProduct Successfully")]
        public async Task Create_CatalogProduct_Successfully()
        {
            var command = new CreateCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                ProductId = this._product.ProductId,
                DisplayName = this._product.Name
            };

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
        }

        [Fact(DisplayName = "Invalid Command Should Throw ValidationException")]
        public async Task Invalid_Command_ShouldThrow_ValidationException()
        {
            var command = new CreateCatalogProductCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty,
                ProductId = (ProductId)Guid.Empty,
                DisplayName = string.Empty
            };

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._requestHandler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Empty Command Should Be Invalid")]
        public void Empty_Command_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty,
                ProductId = (ProductId)Guid.Empty,
                DisplayName = string.Empty
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Catalog Not Found Should Be Invalid")]
        public void Catalog_NotFound_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand
            {
                CatalogId = IdentityFactory.Create<CatalogId>(),
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>(),
                ProductId = this._product.ProductId,
                DisplayName = this._product.Name
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "CatalogCategory Not Found Should Be Invalid")]
        public void CatalogCategory_NotFound_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>(),
                ProductId = this._product.ProductId,
                DisplayName = this._product.Name
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Product Not Found Should Be Invalid")]
        public void Product_NotFound_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                ProductId = IdentityFactory.Create<ProductId>(),
                DisplayName = this._product.Name
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Duplicate Product Should Be Invalid")]
        public void Duplicate_Product_ShouldBe_Invalid()
        {
            var catalogProduct =
                this._catalogCategory.CreateCatalogProduct(this._product.ProductId, this._product.Name);

            var command = new CreateCatalogProductCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                ProductId = this._product.ProductId,
                DisplayName = this._product.Name
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }
    }
}
