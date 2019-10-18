using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogCategory;
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
    /// <summary>
    /// https://github.com/MichalJankowskii/Moq.EntityFrameworkCore
    /// </summary>
    public class TestCreateCatalogCategoryCommand : UnitTestBase<Catalog>
    {
        private readonly Mock<DbContext> _mockDbContext;
        private readonly IRepository<Catalog> _catalogRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly CreateCatalogCategoryCommandValidator _validator;

        public TestCreateCatalogCategoryCommand() : base()
        {
            this._mockDbContext = new Mock<DbContext>();
            this._catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            this._categoryRepository = new DefaultRepositoryAsync<Category>(this._mockDbContext.Object);

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>())
                .Returns(this._catalogRepository);

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Category>())
                .Returns(this._categoryRepository);

            this._validator = new CreateCatalogCategoryCommandValidator(this.MockRepositoryFactory.Object);
        }

        [Fact(DisplayName = "Create CatalogCategory Successfully")]
        public async Task Create_CatalogCategory_Successfully()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> {catalog};

            var category = Category.Create(this.Fixture.Create<string>());
            var categories = new List<Category> {category};
            
            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .ReturnsDbSet(catalogs);
            this._mockDbContext
                .Setup(x => x.Set<Category>())
                .ReturnsDbSet(categories);

            var categoryId = category.CategoryId;
            var catalogId = catalog.CatalogId;

            var command =
                new CreateCatalogCategoryCommand(catalogId.Id, categoryId.Id, this.Fixture.Create<string>());

            IRequestHandler<CreateCatalogCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(catalog), Times.Once);
        }

        [Fact(DisplayName = "Create CatalogCategory As Child Successfully")]
        public async Task Create_CatalogCategory_As_Child_Successfully()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };

            var category1 = Category.Create(this.Fixture.Create<string>());
            var category2 = Category.Create(this.Fixture.Create<string>());
            var categories = new List<Category> { category1, category2 };

            var catalogCategory1 = catalog.AddCategory(category1.CategoryId, category1.DisplayName);

            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .ReturnsDbSet(catalogs);
            this._mockDbContext
                .Setup(x => x.Set<Category>())
                .ReturnsDbSet(categories);

            var command = new CreateCatalogCategoryCommand(catalog.CatalogId.Id,
                                                            category2.CategoryId.Id,
                                                            category2.DisplayName,
                                                            catalogCategory1.CatalogCategoryId.Id);

            IRequestHandler<CreateCatalogCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(It.IsAny<Catalog>()), Times.Once);
        }

        [Fact(DisplayName = "Create CatalogCategory With Fail of Validation Should Throw Exception")]
        public async Task Create_CatalogCategory_With_Fail_Of_Validation_ShouldThrowException()
        {
            var command = new CreateCatalogCategoryCommand(Guid.Empty,
                Guid.Empty,
                string.Empty);

            IRequestHandler<CreateCatalogCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await Should.ThrowAsync<ValidationException>(async () => await handler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Command With Empty Values Should Be Invalid")]
        public void Command_With_Empty_Values_ShouldBeInvalid()
        {
            var command = new CreateCatalogCategoryCommand(Guid.Empty,
                Guid.Empty,
                string.Empty);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Command With Not Found Catalog Should Be Invalid")]
        public void Command_With_NotFound_Catalog_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };

            var category = Category.Create(this.Fixture.Create<string>());
            var categories = new List<Category> { category };

            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .ReturnsDbSet(catalogs);
            this._mockDbContext
                .Setup(x => x.Set<Category>())
                .ReturnsDbSet(categories);

            var categoryId = category.CategoryId;
            var catalogId = catalog.CatalogId;

            var command = new CreateCatalogCategoryCommand(Guid.NewGuid(), catalogId.Id, category.DisplayName);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Command With Not Found Category Should Be Invalid")]
        public void Command_With_NotFound_Category_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };

            var category = Category.Create(this.Fixture.Create<string>());
            var categories = new List<Category> { category };

            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .ReturnsDbSet(catalogs);
            this._mockDbContext
                .Setup(x => x.Set<Category>())
                .ReturnsDbSet(categories);

            var categoryId = category.CategoryId;
            var catalogId = catalog.CatalogId;

            var command = new CreateCatalogCategoryCommand(catalogId.Id, Guid.NewGuid(), category.DisplayName);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Command With Invalid ParentCatalogCategoryId Should Be Invalid")]
        public void Command_With_Invalid_ParentCatalogCategoryId_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };

            var category = Category.Create(this.Fixture.Create<string>());
            var categories = new List<Category> { category };


            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .ReturnsDbSet(catalogs);
            this._mockDbContext
                .Setup(x => x.Set<Category>())
                .ReturnsDbSet(categories);

            var command = new CreateCatalogCategoryCommand(catalog.CatalogId.Id,
                category.CategoryId.Id,
                this.Fixture.Create<string>(),
                Guid.NewGuid());

            var result = this._validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
            result.ShouldHaveValidationErrorFor(x => x.ParentCatalogCategoryId);
        }
    }
}
