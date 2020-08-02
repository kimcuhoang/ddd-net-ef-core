using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => this.ProductMustExist(repositoryFactory, x))
                .WithMessage(x => $"Product#{x.ProductId} could not be found.");

            RuleFor(x => x.ProductName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();
        }

        private bool ProductMustExist(IRepositoryFactory repositoryFactory, ProductId productId)
        {
            var repository = repositoryFactory.CreateRepository<Product, ProductId>();
            var product = repository.FindOneAsync(x => x.Id == productId).Result;
            return product != null;
        }
    }
}
