using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.RemoveCatalogCategory;

public class RemoveCatalogCategoryCommandValidator : AbstractValidator<RemoveCatalogCategoryCommand>
{
    public RemoveCatalogCategoryCommandValidator(IRepository<Catalog, CatalogId> catalogRepository)
    {
        RuleFor(x => x.CatalogId).NotNull().NotEqual(CatalogId.Empty);

        RuleFor(x => x.CatalogCategoryId).NotNull().NotEqual(CatalogCategoryId.Empty);

        When(x => IsValid(x), () =>
        {
            RuleFor(x => x).CustomAsync(async (command, context, token) =>
            {
                var catalogs = catalogRepository.AsQueryable();

                var query =
                    from c in catalogs
                    from c1 in c.Categories.Where(_ => _.Id == command.CatalogCategoryId).DefaultIfEmpty()
                    where c.Id == command.CatalogId
                    select new
                    {
                        Catalog = c,
                        CatalogCategory = c1
                    };

                var result = await query.FirstOrDefaultAsync(token);

                if (result == null)
                {
                    context.AddFailure(nameof(command.CatalogId), $"Could not found Catalog#{command.CatalogId}");
                }
                else if (result.CatalogCategory == null)
                {
                    context.AddFailure(nameof(command.CatalogCategoryId), $"Could not found CatalogCategory#{command.CatalogCategoryId} in Catalog#{command.CatalogId}");
                }
            });
        });
    }

    private readonly Func<RemoveCatalogCategoryCommand, bool> IsValid = command
        => command.CatalogId != null && command.CatalogId != CatalogId.Empty
        && command.CatalogCategoryId != null && command.CatalogCategoryId != CatalogCategoryId.Empty;
}
