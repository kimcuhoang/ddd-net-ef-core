using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;
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

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands
{
    /// <summary>
    /// https://github.com/romantitov/MockQueryable
    /// </summary>
    public class TestCreateCatalogCategoryCommand : UnitTestBase<Catalog>, IAsyncLifetime
    {
        private Mock<DbContext> _mockDbContext;
        private CreateCatalogCategoryCommandValidator _validator;
        private IRequestHandler<CreateCatalogCategoryCommand> _requestHandler;

        private Catalog _catalog;
        private Category _category;
        
        #region Implementation of IAsyncLifetime

        public Task InitializeAsync()
        {
            this._mockDbContext = new Mock<DbContext>();
            var catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            var categoryRepository = new DefaultRepositoryAsync<Category>(this._mockDbContext.Object);

            this.MockRepositoryFactory.Setup(x => x.CreateRepository<Catalog>())
                .Returns(catalogRepository);

            this.MockRepositoryFactory.Setup(x => x.CreateRepository<Category>())
                .Returns(categoryRepository);

            this._catalog = Catalog.Create(this.Fixture.Create<string>());
            this._category = Category.Create(this.Fixture.Create<string>());

            var mockCatalogs = (new List<Catalog> {this._catalog}).AsQueryable().BuildMockDbSet();
            var mockCategories = (new List<Category> {this._category}).AsQueryable().BuildMockDbSet();
            this._mockDbContext.Setup(x => x.Set<Catalog>())
                .Returns(mockCatalogs.Object);
            this._mockDbContext.Setup(x => x.Set<Category>())
                .Returns(mockCategories.Object);

            this._validator = new CreateCatalogCategoryCommandValidator(this.MockRepositoryFactory.Object);
            this._requestHandler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            return Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        [Fact(DisplayName = "Create CatalogCategory Successfully")]
        public async Task Create_CatalogCategory_Successfully()
        {
            var command = new CreateCatalogCategoryCommand
                {
                    CatalogId = this._catalog.CatalogId,
                    CategoryId = this._category.CategoryId,
                    DisplayName = this.Fixture.Create<string>()
                };

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
        }

        [Fact(DisplayName = "Create CatalogCategory As Child Successfully")]
        public async Task Create_CatalogCategory_As_Child_Successfully()
        {
            var catalogCategory = this._catalog.AddCategory(this._category.CategoryId, this._category.DisplayName);
            var childCategory = Category.Create(this.Fixture.Create<string>());

            var mockCategories = (new List<Category> { this._category, childCategory }).AsQueryable().BuildMockDbSet();
            
            this._mockDbContext
                .Setup(x => x.Set<Category>())
                .Returns(mockCategories.Object);

            var command = new CreateCatalogCategoryCommand
            {
                CatalogId = this._catalog.CatalogId,
                CategoryId = childCategory.CategoryId,
                DisplayName = childCategory.DisplayName,
                ParentCatalogCategoryId = catalogCategory.CatalogCategoryId
            };

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
        }

        [Fact(DisplayName = "Create CatalogCategory With Fail of Validation Should Throw Exception")]
        public async Task Create_CatalogCategory_With_Fail_Of_Validation_ShouldThrowException()
        {
            var command = new CreateCatalogCategoryCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CategoryId = (CategoryId)Guid.Empty,
                DisplayName = string.Empty
            };

            await Should.ThrowAsync<ValidationException>(async () => 
                await this._requestHandler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Command With Empty Values Should Be Invalid")]
        public void Command_With_Empty_Values_ShouldBeInvalid()
        {
            var command = new CreateCatalogCategoryCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CategoryId = (CategoryId)Guid.Empty,
                DisplayName = string.Empty
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Command With Not Found Catalog Should Be Invalid")]
        public void Command_With_NotFound_Catalog_ShouldBeInvalid()
        {
            var command = new CreateCatalogCategoryCommand
            {
                CatalogId = IdentityFactory.Create<CatalogId>(),
                CategoryId = this._category.CategoryId,
                DisplayName = this._category.DisplayName
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Command With Not Found Category Should Be Invalid")]
        public void Command_With_NotFound_Category_ShouldBeInvalid()
        {
            var command = new CreateCatalogCategoryCommand
            {
                CatalogId = this._catalog.CatalogId,
                CategoryId = IdentityFactory.Create<CategoryId>(),
                DisplayName = this.Fixture.Create<string>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Command With Invalid ParentCatalogCategoryId Should Be Invalid")]
        public void Command_With_Invalid_ParentCatalogCategoryId_ShouldBeInvalid()
        {

            var command = new CreateCatalogCategoryCommand
            {
                CatalogId = this._catalog.CatalogId,
                CategoryId = this._category.CategoryId,
                DisplayName = this.Fixture.Create<string>(),
                ParentCatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
            result.ShouldHaveValidationErrorFor(x => x.ParentCatalogCategoryId);
        }
    }
}
