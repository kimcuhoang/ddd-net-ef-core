using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands
{
    public class TestCreateCatalogCommand : UnitTestBase<Catalog>
    {
        [Theory(DisplayName = "Create Catalog Without CatalogCategory Successfully")]
        [AutoData]
        public async Task Create_Catalog_Without_CatalogCategory_Successfully(string catalogName)
        {
            var command = new CreateCatalogCommand(catalogName);

            IRequestHandler<CreateCatalogCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new CreateCatalogCommandValidator());

            await handler.Handle(command, this.CancellationToken);

            this.MockRepository.Verify(x => x.AddAsync(It.IsAny<Catalog>()), Times.Once);
        }

        [Theory(DisplayName = "Create Catalog With CatalogCategories Successfully")]
        [AutoData]
        public async Task Create_Catalog_With_CatalogCategories_Successfully(string catalogName)
        {
            var command = new CreateCatalogCommand(catalogName);
            for (var i = 0; i < 5; i++)
            {
                command.AddCategory(Guid.NewGuid(), this.Fixture.Create<string>());
            }

            IRequestHandler<CreateCatalogCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new CreateCatalogCommandValidator());

            await handler.Handle(command, this.CancellationToken);

            this.MockRepository.Verify(x => x.AddAsync(It.IsAny<Catalog>()), Times.Once);
        }

        [Theory(DisplayName = "Create Catalog With Invalid Command Should Throw Exception")]
        [AutoData]
        public async Task Create_Catalog_With_Invalid_Command_ShouldThrowException(string catalogName)
        {
            var command = new CreateCatalogCommand(string.Empty);
            command.AddCategory(Guid.Empty, string.Empty);

            IRequestHandler<CreateCatalogCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new CreateCatalogCommandValidator());

            await Should.ThrowAsync<ValidationException>(async () =>
                await handler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "CreateCatalogCommand With Empty CatalogName Should Be Invalid")]
        public void CreateCatalogCommand_With_Empty_CatalogName_ShouldBeInvalid()
        {
            var command = new CreateCatalogCommand(string.Empty);
            var validator = new CreateCatalogCommandValidator();

            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CatalogName);
            result.ShouldNotHaveValidationErrorFor(
                $"{nameof(command.Categories)}[0].{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
            result.ShouldNotHaveValidationErrorFor(
                $"{nameof(command.Categories)}[0].{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
        }

        [Fact(DisplayName = "CreateCatalogCommand has Categories Without Id and DisplayName Should Be Invalid")]
        public void CreateCatalogCommand_Has_Categories_With_Empty_Id_And_DisplayName_ShouldBeInvalid()
        {
            var command = new CreateCatalogCommand(this.Fixture.Create<string>());
            command.AddCategory(Guid.Empty, string.Empty);

            var validator = new CreateCatalogCommandValidator();

            var result = validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.CatalogName);
            
            // To verify each item in array, the 'PropertyName' must be for example Categories.DisplayName
            
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
        }
    }
}
