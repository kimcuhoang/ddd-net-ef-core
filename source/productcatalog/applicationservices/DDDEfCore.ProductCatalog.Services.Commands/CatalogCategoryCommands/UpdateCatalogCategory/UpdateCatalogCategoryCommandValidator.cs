using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;

public class UpdateCatalogCategoryCommandValidator : AbstractValidator<UpdateCatalogCategoryCommand>
{
    public UpdateCatalogCategoryCommandValidator(IRepository<Catalog, CatalogId> catalogRepository)
    {
        RuleFor(x => x.CatalogId).NotNull();

        RuleFor(x => x.CatalogCategoryId).NotNull();

        When(x => x.CatalogId != null && x.CatalogCategoryId != null, () =>
        {
            RuleFor(x => x).CustomAsync(async (x, context, token) =>
            {
                var catalogs = catalogRepository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories.Where(_ => _.Id == x.CatalogCategoryId).DefaultIfEmpty()
                    where c.Id == x.CatalogId
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

                var result = await query.FirstOrDefaultAsync(token);

                if (result == null)
                {
                    context.AddFailure(nameof(x.CatalogId), $"Catalog#{x.CatalogId} could not be found");
                }
                else if(result.CatalogCategory == null)
                {
                    context.AddFailure(nameof(x.CatalogCategoryId),
                        $"CatalogCategory#{x.CatalogCategoryId} could not be found in Catalog#{x.CatalogId}");
                }
            });
        });

        RuleFor(x => x.DisplayName).NotNull().NotEmpty();
    }
}
