﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct;

public class CreateCatalogProductCommandValidator : AbstractValidator<CreateCatalogProductCommand>
{
    public CreateCatalogProductCommandValidator(IRepository<Catalog, CatalogId> catalogRepository,
                                                IRepository<Product, ProductId> productRepository)
    {
        RuleFor(x => x.CatalogId).NotNull();

        RuleFor(x => x.CatalogCategoryId).NotNull();

        RuleFor(x => x.ProductId)
            .NotNull()
            .MustAsync(async (x, token) => await this.ProductMustExist(productRepository, x))
            .WithMessage(x => $"Product#{x.ProductId} could not be found.");

        When(this.CommandIsValid, () =>
        {
            RuleFor(command => command).CustomAsync(async (command, context, token) =>
            {
                var catalogs = catalogRepository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories.Where(_ => _.Id == command.CatalogCategoryId).DefaultIfEmpty()
                    let p = c1.Products.FirstOrDefault(_ => _.Id == command.ProductId)
                    where c.Id == command.CatalogId
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1,
                        CatalogProduct = p
                    };

                var result = await query.FirstOrDefaultAsync(token);

                if (result == null)
                {
                    context.AddFailure($"{nameof(command.CatalogId)}", $"Catalog#{command.CatalogId} could not be found.");
                }
                else if (result.CatalogCategory == null)
                {
                    context.AddFailure($"{nameof(command.CatalogCategoryId)}",
                        $"CatalogCategory#{command.CatalogCategoryId} could not be found in Catalog#{command.CatalogId}.");
                }
                else if (result.CatalogProduct == null)
                {
                    context.AddFailure($"{nameof(command.ProductId)}",
                            $"Product#{command.ProductId} is existing in CatalogCategory#{command.CatalogCategoryId}.");
                }
            });
        });

        RuleFor(x => x.DisplayName)
            .NotNull()
            .NotEmpty();
    }

    private bool CommandIsValid(CreateCatalogProductCommand command)
    {
        return command.CatalogId != null && command.CatalogCategoryId != null && command.ProductId != null;
    }

    private async Task<bool> ProductMustExist(IRepository<Product, ProductId> productRepository, ProductId productId)
    {
        var product = await productRepository.FindOneAsync(x => x.Id == productId);
        return product != null;
    }
}
