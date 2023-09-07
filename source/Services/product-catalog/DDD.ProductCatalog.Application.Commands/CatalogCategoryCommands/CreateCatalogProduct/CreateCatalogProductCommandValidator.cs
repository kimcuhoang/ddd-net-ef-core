using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.CreateCatalogProduct;

public class CreateCatalogProductCommandValidator : AbstractValidator<CreateCatalogProductCommand>
{
    public CreateCatalogProductCommandValidator(IRepository<Catalog, CatalogId> catalogRepository,
                                                IRepository<Product, ProductId> productRepository)
    {
        RuleFor(x => x.CatalogId).NotNull().NotEqual(CatalogId.Empty);

        RuleFor(x => x.CatalogCategoryId).NotNull().NotEqual(CatalogCategoryId.Empty);

        RuleFor(x => x.ProductId)
            .NotNull().NotEqual(ProductId.Empty)
            .MustAsync(async (x, token) => await this.ProductMustExist(productRepository, x))
            .WithMessage(x => $"Product#{x.ProductId} could not be found.");

        RuleFor(x => x.DisplayName)
            .NotNull()
            .NotEmpty();

        When(this.CommandIsValid, () =>
        {
            RuleFor(command => command).CustomAsync(async (command, context, token) =>
            {
                var catalogs = catalogRepository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories.Where(_ => _.Id == command.CatalogCategoryId).DefaultIfEmpty()
                    let p = c1 != null
                                ? c1.Products.FirstOrDefault(_ => _.ProductId == command.ProductId)
                                : null
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
                else if (result.CatalogCategory is null)
                {
                    context.AddFailure($"{nameof(command.CatalogCategoryId)}",
                        $"CatalogCategory#{command.CatalogCategoryId} could not be found in Catalog#{command.CatalogId}.");
                }
                else if (result.CatalogProduct is not null)
                {
                    context.AddFailure($"{nameof(command.ProductId)}",
                            $"Product#{command.ProductId} is existing in CatalogCategory#{command.CatalogCategoryId}.");
                }
            });
        });


    }

    private bool CommandIsValid(CreateCatalogProductCommand command)
    {
        return command.CatalogId is not null && command.CatalogId != CatalogId.Empty
            && command.CatalogCategoryId is not null && command.CatalogCategoryId != CatalogCategoryId.Empty
            && command.ProductId is not null && command.ProductId != ProductId.Empty;
    }

    private async Task<bool> ProductMustExist(IRepository<Product, ProductId> productRepository, ProductId productId)
    {
        var product = await productRepository.FindOneAsync(x => x.Id == productId);
        return product is not null;
    }
}
