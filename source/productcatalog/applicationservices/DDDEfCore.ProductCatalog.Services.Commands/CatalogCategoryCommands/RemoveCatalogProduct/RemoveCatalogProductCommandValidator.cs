using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct;

public class RemoveCatalogProductCommandValidator : AbstractValidator<RemoveCatalogProductCommand>
{
    public RemoveCatalogProductCommandValidator(IRepository<Catalog, CatalogId> catalogRepository)
    {
        RuleFor(x => x.CatalogId).NotNull().Must(_ => _ != CatalogId.Empty);

        RuleFor(x => x.CatalogCategoryId).NotNull().Must(_ => _ != CatalogCategoryId.Empty);

        RuleFor(x => x.CatalogProductId).NotNull().Must(_ => _ != CatalogProductId.Empty);

        When(CommandIsValid, () =>
        {
            RuleFor(command => command).CustomAsync(async (command, context, token) =>
            {
                var catalogs = catalogRepository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories
                                .Where(_ => _.Id == command.CatalogCategoryId)
                                .DefaultIfEmpty()
                    let p = c1 != null 
                            ? c1.Products.Where(_ => _.Id == command.CatalogProductId).FirstOrDefault()
                            : null
                    where c.Id == command.CatalogId
                    select new
                    {
                        Catalog = c,
                        CatalogProduct = p,
                        CatalogCategory = c1
                    };

                var result = await query.FirstOrDefaultAsync(token);


                if (result == null)
                {
                    context.AddFailure(nameof(command.CatalogId), $"Catalog#{command.CatalogId} could not be found.");
                }
                else if (result.CatalogCategory == null)
                {
                    context.AddFailure(nameof(command.CatalogCategoryId), $"CatalogCategory#{command.CatalogCategoryId} could not be found in Catalog#{command.CatalogId}");
                }
                else if (result.CatalogProduct == null)
                {
                    context.AddFailure(nameof(command.CatalogProductId), $"CatalogProduct#{command.CatalogProductId} could not be found in CatalogCategory#{command.CatalogCategoryId} of Catalog#{command.CatalogId}");
                }
            });
        });
    }

    private bool CommandIsValid(RemoveCatalogProductCommand command)
    {
        return command.CatalogId != null && command.CatalogId != CatalogId.Empty
            && command.CatalogCategoryId != null && command.CatalogCategoryId != CatalogCategoryId.Empty
            && command.CatalogProductId != null && command.CatalogProductId != CatalogProductId.Empty;
    }
}
