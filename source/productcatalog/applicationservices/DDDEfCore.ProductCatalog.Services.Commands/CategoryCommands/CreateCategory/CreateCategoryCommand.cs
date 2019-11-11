using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory
{
    public sealed class CreateCategoryCommand : IRequest
    {
        public string CategoryName { get; set; }

        public CreateCategoryCommand() { }

        public CreateCategoryCommand(string categoryName) : this()
        {
            this.CategoryName = categoryName;
        }
    }
}
