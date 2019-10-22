using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands
{
    public class TestCreateCatalogCommand : UnitTestBase<Catalog>
    {
        private readonly CreateCatalogCommandValidator _validator;

        public TestCreateCatalogCommand() : base()
        {
            this._validator = new CreateCatalogCommandValidator(this.MockRepositoryFactory.Object);
        }

        [Theory(DisplayName = "Create Catalog Without CatalogCategory Successfully")]
        [AutoData]
        public async Task Create_Catalog_Without_CatalogCategory_Successfully(string catalogName)
        {
            var command = new CreateCatalogCommand(catalogName);

            IRequestHandler<CreateCatalogCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this.MockRepository.Verify(x => x.AddAsync(It.IsAny<Catalog>()), Times.Once);
        }

        [Theory(DisplayName = "Create Catalog With CatalogCategories Successfully")]
        [AutoData]
        public async Task Create_Catalog_With_CatalogCategories_Successfully(string catalogName)
        {
            var mockDbContext = new Mock<DbContext>();
            var categories = new List<Category>();
            Enumerable.Range(0, 4).ToList().ForEach(idx =>
            {
                categories.Add(Category.Create(this.Fixture.Create<string>()));
            });
            mockDbContext.Setup(x => x.Set<Category>()).ReturnsDbSet(categories);
            var categoryRepository = new DefaultRepositoryAsync<Category>(mockDbContext.Object);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Category>())
                .Returns(categoryRepository);

            var command = new CreateCatalogCommand(catalogName);
            foreach (var category in categories)
            {
                command.AddCategory(category.CategoryId.Id, this.Fixture.Create<string>());
            }

            IRequestHandler<CreateCatalogCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this.MockRepository.Verify(x => x.AddAsync(It.IsAny<Catalog>()), Times.Once);
        }

        [Fact(DisplayName = "Create Catalog With Invalid Command Should Throw Exception")]
        public async Task Create_Catalog_With_Invalid_Command_ShouldThrowException()
        {
            var command = new CreateCatalogCommand(string.Empty);
            command.AddCategory(Guid.Empty, string.Empty);

            IRequestHandler<CreateCatalogCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await Should.ThrowAsync<ValidationException>(async () =>
                await handler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "CreateCatalogCommand With Empty CatalogName Should Be Invalid")]
        public void CreateCatalogCommand_With_Empty_CatalogName_ShouldBeInvalid()
        {
            var command = new CreateCatalogCommand(string.Empty);

            var result = this._validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CatalogName);
            result.ShouldNotHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
            result.ShouldNotHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
        }

        [Fact(DisplayName = "CreateCatalogCommand has Categories Without Id and DisplayName Should Be Invalid")]
        public void CreateCatalogCommand_Has_Categories_With_Empty_Id_And_DisplayName_ShouldBeInvalid()
        {
            var command = new CreateCatalogCommand(this.Fixture.Create<string>());
            command.AddCategory(Guid.Empty, string.Empty);

            var result = this._validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.CatalogName);
            
            // To verify each item in array, the 'PropertyName' must be for example Categories.DisplayName
            
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
        }
    }
}
