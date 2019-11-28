using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public sealed class UpdateCategoryCommand : IRequest
    {
        public CategoryId CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
