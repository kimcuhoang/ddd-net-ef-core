using AutoFixture;
using DDDEfCore.Core.Common;
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
    public class TestCreateCatalogProductCommand : UnitTestBase<Catalog>
    {
        private readonly Mock<DbContext> _mockDbContext;
        private readonly IRepository<Catalog> _catalogRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly CreateCatalogProductCommandValidator _validator;
        private readonly IRequestHandler<CreateCatalogProductCommand> _requestHandler;

        private readonly Catalog _catalog;
        private readonly Category _category;
        private readonly Product _product;
        private readonly CatalogCategory _catalogCategory;

        public TestCreateCatalogProductCommand()
        {
            this._mockDbContext = new Mock<DbContext>();
            this._catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            this._productRepository = new DefaultRepositoryAsync<Product>(this._mockDbContext.Object);
            this._validator = new CreateCatalogProductCommandValidator(this.MockRepositoryFactory.Object);
            

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>()).Returns(this._catalogRepository);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Product>()).Returns(this._productRepository);

            this._catalog = Catalog.Create(this.Fixture.Create<string>());
            this._category = Category.Create(this.Fixture.Create<string>());
            this._product = Product.Create(this.Fixture.Create<string>());
            this._catalogCategory = this._catalog.AddCategory(this._category.CategoryId, this._category.DisplayName);

            var catalogs = new List<Catalog> { this._catalog };
            var products = new List<Product> { this._product };

            this._mockDbContext
                .Setup(x => x.Set<Catalog>()).ReturnsDbSet(catalogs);
            this._mockDbContext
                .Setup(x => x.Set<Product>()).ReturnsDbSet(products);
            

            this._requestHandler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);
        }

        [Fact(DisplayName = "Create CatalogProduct Successfully")]
        public async Task Create_CatalogProduct_Successfully()
        {
            var command = new CreateCatalogProductCommand(this._catalog.CatalogId.Id,
                this._catalogCategory.CatalogCategoryId.Id,
                this._product.ProductId.Id,
                this._product.Name);

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
        }

        [Fact(DisplayName = "Invalid Command Should Throw ValidationException")]
        public async Task Invalid_Command_ShouldThrow_ValidationException()
        {
            var command = new CreateCatalogProductCommand(Guid.Empty,
                Guid.Empty, 
                Guid.Empty,
                string.Empty);

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._requestHandler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Empty Command Should Be Invalid")]
        public void Empty_Command_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand(Guid.Empty,
                Guid.Empty,
                Guid.Empty,
                string.Empty);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Catalog Not Found Should Be Invalid")]
        public void Catalog_NotFound_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand(Guid.NewGuid(),
                Guid.NewGuid(),
                this._product.ProductId.Id,
                this._product.Name);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "CatalogCategory Not Found Should Be Invalid")]
        public void CatalogCategory_NotFound_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand(this._catalog.CatalogId.Id,
                Guid.NewGuid(),
                this._product.ProductId.Id,
                this._product.Name);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Product Not Found Should Be Invalid")]
        public void Product_NotFound_ShouldBe_Invalid()
        {
            var command = new CreateCatalogProductCommand(this._catalog.CatalogId.Id,
                this._catalogCategory.CatalogCategoryId.Id,
                Guid.NewGuid(),
                this._product.Name);

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

            var command = new CreateCatalogProductCommand(this._catalog.CatalogId.Id,
                this._catalogCategory.CatalogCategoryId.Id,
                this._product.ProductId.Id,
                this._product.Name);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }
    }
}
