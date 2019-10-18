using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;
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
    public class TestUpdateCatalogCategoryCommand : UnitTestBase<Catalog>
    {
        private readonly Mock<DbContext> _mockDbContext;
        private readonly IRepository<Catalog> _catalogRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly UpdateCatalogCategoryCommandValidator _validator;

        public TestUpdateCatalogCategoryCommand() : base()
        {
            this._mockDbContext = new Mock<DbContext>();
            this._catalogRepository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            this._categoryRepository = new DefaultRepositoryAsync<Category>(this._mockDbContext.Object);
            this._validator = new UpdateCatalogCategoryCommandValidator(this.MockRepositoryFactory.Object);

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>())
                .Returns(this._catalogRepository);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Category>())
                .Returns(this._categoryRepository);
        }

        [Fact(DisplayName = "Update CatalogCategory Successfully")]
        public async Task Update_CatalogCategory_Successfully()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var category = Category.Create(this.Fixture.Create<string>());
            var catalogCategory = catalog.AddCategory(category.CategoryId, category.DisplayName);
            var catalogs = new List<Catalog> {catalog};
            var categories = new List<Category> {category};

            this._mockDbContext.Setup(x => x.Set<Catalog>()).ReturnsDbSet(catalogs);
            this._mockDbContext.Setup(x => x.Set<Category>()).ReturnsDbSet(categories);

            var catalogId = catalog.CatalogId.Id;
            var catalogCategoryId = catalogCategory.CatalogCategoryId.Id;

            var command = new UpdateCatalogCategoryCommand(catalogId, catalogCategoryId, this.Fixture.Create<string>());

            IRequestHandler<UpdateCatalogCategoryCommand> handler =
                new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(It.IsAny<Catalog>()), Times.Once);
        }

        [Fact(DisplayName = "Validate Fail Should Throw Exception")]
        public async Task Validation_Fail_ShouldThrowException()
        {
            var command = new UpdateCatalogCategoryCommand(Guid.Empty, Guid.Empty, this.Fixture.Create<string>());

            IRequestHandler<UpdateCatalogCategoryCommand> handler =
                new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await Should.ThrowAsync<ValidationException>(async () =>
                await handler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Not Found Catalog Should Be Invalid")]
        public void Not_Found_Catalog_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> {catalog};

            this._mockDbContext.Setup(x => x.Set<Catalog>()).ReturnsDbSet(catalogs);

            var command = new UpdateCatalogCategoryCommand(Guid.NewGuid(), Guid.NewGuid(), this.Fixture.Create<string>());

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Not Found CatalogCategory Should Be Invalid")]
        public void Not_Found_CatalogCategory_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };

            this._mockDbContext.Setup(x => x.Set<Catalog>()).ReturnsDbSet(catalogs);

            var command = new UpdateCatalogCategoryCommand(catalog.CatalogId.Id, Guid.NewGuid(), this.Fixture.Create<string>());

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact(DisplayName = "Empty DisplayName Should Be Invalid")]
        public void Empty_DisplayName_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this.Fixture.Create<string>());

            this._mockDbContext.Setup(x => x.Set<Catalog>()).ReturnsDbSet(catalogs);

            var command = new UpdateCatalogCategoryCommand(catalog.CatalogId.Id, 
                                                        catalogCategory.CatalogCategoryId.Id, 
                                                        string.Empty);

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        }
    }
}
