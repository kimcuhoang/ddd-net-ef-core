using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;
using FluentValidation;
using MediatR;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatagoryCommands
{
    public class TestCreateCategoryCommand : UnitTestBase<Category>
    {
        [Fact(DisplayName = "Create Category Successfully")]
        public async Task Create_Category_Successfully()
        {
            var command = this.Fixture.Create<CreateCategoryCommand>();

            IRequestHandler<CreateCategoryCommand> handler 
                = new CommandHandler(this.MockRepositoryFactory.Object, new CreateCategoryCommandValidator());

            await handler.Handle(command, this.CancellationToken);

            this.MockRepository.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact(DisplayName = "Create Category With Empty Name Should Throw Exception")]
        public async Task Create_Category_With_Empty_Name_ShouldThrowException()
        {
            var command = new CreateCategoryCommand(string.Empty);
            IRequestHandler<CreateCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, new CreateCategoryCommandValidator());

            await Should.ThrowAsync<ValidationException>(async () => await handler.Handle(command, this.CancellationToken));
            this.MockRepository.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact(DisplayName = "CreateCategoryCommand Has Empty CategoryName Should Be Invalid")]
        public void CreateCategoryCommand_Has_Empty_CategoryName_ShouldBeInvalid()
        {
            var command = new CreateCategoryCommand(string.Empty);
            
            var validator = new CreateCategoryCommandValidator();
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CategoryName);
        }
    }
}
