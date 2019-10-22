using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public sealed class UpdateCategoryCommand : IRequest
    {
        public CategoryId CategoryId { get; }
        public string CategoryName { get; }

        public UpdateCategoryCommand(Guid categoryId, string categoryName)
        {
            this.CategoryId = new CategoryId(categoryId);
            this.CategoryName = categoryName;
        }
    }
}
