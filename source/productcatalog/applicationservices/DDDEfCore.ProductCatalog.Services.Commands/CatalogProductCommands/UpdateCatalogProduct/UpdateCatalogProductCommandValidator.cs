using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogProductCommands.UpdateCatalogProduct;

public class UpdateCatalogProductCommandValidator : AbstractValidator<UpdateCatalogProductCommand>
{
    public UpdateCatalogProductCommandValidator(IRepository<Catalog, CatalogId> catalogRepository)
    {
        RuleFor(x => x.CatalogId).NotNull();

        RuleFor(x => x.CatalogCategoryId).NotNull();

        RuleFor(x => x.CatalogProductId).NotNull();

        When(CommandIsValid, () =>
        {
            RuleFor(command => command).CustomAsync(async (command, context, token) =>
            {
                var catalogs = catalogRepository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories.Where(_ => _.Id == command.CatalogCategoryId).DefaultIfEmpty()
                    let p = c1 != null
                                ? c1.Products.FirstOrDefault(_ => _.Id == command.CatalogProductId)
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
                    context.AddFailure(nameof(command.CatalogId), $"Catalog#{command.CatalogId} could not be found.");
                }
                else if(result.CatalogCategory == null)
                {
                    context.AddFailure(nameof(command.CatalogCategoryId), $"CatalogCategory#{command.CatalogCategoryId} could not be found in Catalog#{command.CatalogId}");
                }
                else if(result.CatalogProduct == null)
                {
                    context.AddFailure(nameof(command.CatalogProductId),
                        $"CatalogProduct#{command.CatalogProductId} could not be found in CatalogCategory#{command.CatalogCategoryId} of Catalog#{command.CatalogId}");
                }

            });
        });

        RuleFor(x => x.DisplayName).NotNull().NotEmpty();
    }

    private bool CommandIsValid(UpdateCatalogProductCommand command)
    {
        return command.CatalogId != null && command.CatalogId != CatalogId.Empty
            && command.CatalogCategoryId != null && command.CatalogCategoryId != CatalogCategoryId.Empty
            && command.CatalogProductId != null && command.CatalogProductId != CatalogProductId.Empty;
    }
}
