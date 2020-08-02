using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Moq;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCategoryCommands
{
    public class TestUpdateCategoryCommand : UnitTestBase<Category, CategoryId>
    {
        private readonly UpdateCategoryCommandValidator _validator;

        public TestUpdateCategoryCommand() : base()
        {
            this._validator = new UpdateCategoryCommandValidator(this.MockRepositoryFactory.Object);
            
        }

        [Fact(DisplayName = "Update Category Successfully")]
        public async Task Update_Category_Successfully()
        {
            var category = Category.Create(this.Fixture.Create<string>());

            this.MockRepository
                .Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(category);

            var command = new UpdateCategoryCommand
            {
                CategoryId = category.Id,
                CategoryName = this.Fixture.Create<string>()
            };

            IRequestHandler<UpdateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            category.DisplayName.ShouldBe(command.CategoryName);
            this.MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact(DisplayName = "Update Not Found Category Should Throw Exception")]
        public async Task Update_NotFound_Category_ShouldThrowException()
        {
            var command = new UpdateCategoryCommand
            {
                CategoryId = IdentityFactory.Create<CategoryId>(),
                CategoryName = this.Fixture.Create<string>()
            };

            this.MockRepository
                .Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync((Category)null);

            IRequestHandler<UpdateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await Should.ThrowAsync<ValidationException>(async () => await handler.Handle(command, this.CancellationToken));
        }

        [Fact(DisplayName = "Update Category With Invalid Command Should Throw Exception")]
        public async Task Update_Category_With_InvalidCommand_ShouldThrowException()
        {
            var command = new UpdateCategoryCommand
            {
                CategoryId = (CategoryId)Guid.Empty,
                CategoryName = string.Empty
            };

            IRequestHandler<UpdateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await Should.ThrowAsync<ValidationException>(async () => await handler.Handle(command, this.CancellationToken));
            this.MockRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact(DisplayName = "UpdateCategoryCommand With Empty Values (Id, Name) Should Be Invalid")]
        public void UpdateCategoryCommand_With_Empty_Values_ShouldBeInvalid()
        {
            var command = new UpdateCategoryCommand
            {
                CategoryId = (CategoryId)Guid.Empty,
                CategoryName = string.Empty
            };

            var validationResult = this._validator.TestValidate(command);
            validationResult.ShouldHaveValidationErrorFor(x => x.CategoryId);
            validationResult.ShouldHaveValidationErrorFor(x => x.CategoryName);
        }
    }
}
