using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;
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
    public class TestUpdateCatalogCategoryCommand : UnitTestBase<Catalog>, IAsyncLifetime
    {
        private Mock<DbContext> _mockDbContext;
        private UpdateCatalogCategoryCommandValidator _validator;
        private IRequestHandler<UpdateCatalogCategoryCommand> _requestHandler;

        private Catalog _catalog;
        private Category _category;
        private CatalogCategory _catalogCategory;

        #region Implementation of IAsyncLifetime

        public Task InitializeAsync()
        {
            this._mockDbContext = new Mock<DbContext>();
            var catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            var categoryRepository = new DefaultRepositoryAsync<Category>(this._mockDbContext.Object);
            
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>())
                .Returns(catalogRepository);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Category>())
                .Returns(categoryRepository);

            this._catalog = Catalog.Create(this.Fixture.Create<string>());
            this._category = Category.Create(this.Fixture.Create<string>());
            this._catalogCategory = this._catalog.AddCategory(this._category.CategoryId, this._category.DisplayName);

            this._mockDbContext.Setup(x => x.Set<Catalog>())
                .Returns((new List<Catalog> { this._catalog }).AsQueryable().BuildMockDbSet().Object);
            this._mockDbContext.Setup(x => x.Set<Category>())
                .Returns((new List<Category> { this._category }).AsQueryable().BuildMockDbSet().Object);

            this._validator = new UpdateCatalogCategoryCommandValidator(this.MockRepositoryFactory.Object);
            this._requestHandler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            return Task.CompletedTask;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        [Fact(DisplayName = "Update CatalogCategory Successfully")]
        public async Task Update_CatalogCategory_Successfully()
        {
            var command = new UpdateCatalogCategoryCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                DisplayName = this.Fixture.Create<string>()
            };

            await this._requestHandler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(It.IsAny<Catalog>()), Times.Once);
        }

        [Fact(DisplayName = "Validate Fail Should Throw Exception")]
        public async Task Validation_Fail_ShouldThrowException()
        {
            var command = new UpdateCatalogCategoryCommand
            {
                CatalogId = (CatalogId)Guid.Empty,
                CatalogCategoryId = (CatalogCategoryId)Guid.Empty,
                DisplayName = this.Fixture.Create<string>()
            };

            await Should.ThrowAsync<ValidationException>(async () =>
                await this._requestHandler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Not Found Catalog Should Be Invalid")]
        public void Not_Found_Catalog_ShouldBeInvalid()
        {
            var command = new UpdateCatalogCategoryCommand
            {
                CatalogId = IdentityFactory.Create<CatalogId>(),
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>(),
                DisplayName = this.Fixture.Create<string>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Not Found CatalogCategory Should Be Invalid")]
        public void Not_Found_CatalogCategory_ShouldBeInvalid()
        {
            var command = new UpdateCatalogCategoryCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>(),
                DisplayName = this.Fixture.Create<string>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Empty DisplayName Should Be Invalid")]
        public void Empty_DisplayName_ShouldBeInvalid()
        {
            var command = new UpdateCatalogCategoryCommand
            {
                CatalogId = this._catalog.CatalogId,
                CatalogCategoryId = this._catalogCategory.CatalogCategoryId,
                DisplayName = string.Empty
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        }

        
    }
}
