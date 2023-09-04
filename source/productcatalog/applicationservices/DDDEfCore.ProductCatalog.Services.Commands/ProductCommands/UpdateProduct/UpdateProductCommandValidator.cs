﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(IRepository<Product, ProductId> productRepository)
    {
        RuleFor(x => x.ProductId)
            .NotNull()
            .MustAsync(async (x, token) => await this.ProductMustExist(productRepository, x))
            .WithMessage(x => $"Product#{x.ProductId} could not be found.");

        RuleFor(x => x.ProductName)
            .NotNull()
            .NotEmpty();
    }

    private async Task<bool> ProductMustExist(IRepository<Product, ProductId> productRepository, ProductId productId)
    {
        var product = await productRepository.FindOneAsync(x => x.Id == productId);
        return product != null;
    }
}
