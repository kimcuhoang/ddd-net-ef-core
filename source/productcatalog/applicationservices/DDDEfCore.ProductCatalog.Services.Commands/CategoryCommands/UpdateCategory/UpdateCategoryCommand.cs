using System;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public sealed class UpdateCategoryCommand : IRequest
    {
        public Guid CategoryId { get; }
        public string CategoryName { get; }

        public UpdateCategoryCommand(Guid categoryId, string categoryName)
        {
            this.CategoryId = categoryId;
            this.CategoryName = categoryName;
        }
    }
}
