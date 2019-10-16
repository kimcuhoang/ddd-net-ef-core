using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;
using DDDEfCore.ProductCatalog.Services.Commands.Exceptions;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatagoryCommands
{
    public class TestUpdateCategoryCommand : UnitTestBase<Category>
    {
        [Fact(DisplayName = "Update Category Successfully")]
        public async Task Update_Category_Successfully()
        {
            var category = Category.Create(this.Fixture.Create<string>());

            this.MockRepository
                .Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(category);

            var command = new UpdateCategoryCommand(category.CategoryId.Id, this.Fixture.Create<string>());

            IRequestHandler<UpdateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new UpdateCategoryCommandValidator());

            await handler.Handle(command, this.CancellationToken);

            category.DisplayName.ShouldBe(command.CategoryName);
            this.MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact(DisplayName = "Update Not Found Category Should Throw Exception")]
        public async Task Update_NotFound_Category_ShouldThrowException()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), this.Fixture.Create<string>());

            IRequestHandler<UpdateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new UpdateCategoryCommandValidator());

            await Should.ThrowAsync<NotFoundEntityException>(async () =>
                await handler.Handle(command, this.CancellationToken));
            this.MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact(DisplayName = "Update Category With Invalid Command Should Throw Exception")]
        public async Task Update_Category_With_InvalidCommand_ShouldThrowException()
        {
            var command = new UpdateCategoryCommand(Guid.Empty, string.Empty);

            IRequestHandler<UpdateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new UpdateCategoryCommandValidator());

            await Should.ThrowAsync<ValidationException>(async () =>
                await handler.Handle(command, this.CancellationToken));
            this.MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact(DisplayName = "UpdateCategoryCommand With Empty Values (Id, Name) Should Be Invalid")]
        public void UpdateCategoryCommand_With_Empty_Values_ShouldBeInvalid()
        {
            var command = new UpdateCategoryCommand(Guid.Empty, string.Empty);

            var validator = new UpdateCategoryCommandValidator();

            var validationResult = validator.TestValidate(command);
            validationResult.ShouldHaveValidationErrorFor(x => x.CategoryId);
            validationResult.ShouldHaveValidationErrorFor(x => x.CategoryName);
        }
    }
}
