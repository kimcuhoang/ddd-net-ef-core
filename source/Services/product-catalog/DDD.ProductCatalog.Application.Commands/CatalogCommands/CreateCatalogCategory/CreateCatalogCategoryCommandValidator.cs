using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalogCategory;

public class CreateCatalogCategoryCommandValidator : AbstractValidator<CreateCatalogCategoryCommand>
{
    public CreateCatalogCategoryCommandValidator(IRepository<Catalog, CatalogId> catalogRepository,
                                                IRepository<Category, CategoryId> categoryRespository)
    {
        RuleFor(x => x.CatalogId)
            .NotNull().NotEqual(CatalogId.Empty)
            .MustAsync(async (x, token) => await this.CatalogIsExisting(catalogRepository, x))
            .WithMessage(x => $"Catalog#{x.CatalogId} could not be found.");

        RuleFor(x => x.CategoryId)
            .NotNull().NotEqual(CategoryId.Empty)
            .MustAsync(async (x, token) => await this.CategoryIsExisting(categoryRespository, x))
            .WithMessage(x => $"Category#{x.CategoryId} could not be found.");

        RuleFor(x => x.DisplayName)
            .NotNull()
            .NotEmpty();

        When(x => x.ParentCatalogCategoryId is not null, () =>
        {
            RuleFor(x => x).CustomAsync(async (x, context, token) =>
            {
                var categoryWasAdded = await this.CatalogCategoryIsExistingInCatalog(catalogRepository, x.CatalogId, x.ParentCatalogCategoryId);

                if (!categoryWasAdded)
                {
                    context.AddFailure(nameof(CreateCatalogCategoryCommand.ParentCatalogCategoryId), $"Could not found in Catalog#{x.CatalogId}");
                }
            });
        });
    }

    private async Task<bool> CatalogIsExisting(IRepository<Catalog, CatalogId> catalogRepository, CatalogId catalogId)
    {
        var catalog = await catalogRepository.FindOneAsync(x => x.Id == catalogId);
        return catalog is not null;
    }

    private async Task<bool> CategoryIsExisting(IRepository<Category, CategoryId> categoryRepository, CategoryId categoryId)
    {
        var category = await categoryRepository.FindOneAsync(x => x.Id == categoryId);
        return category is not null;
    }

    private async Task<bool> CatalogCategoryIsExistingInCatalog(IRepository<Catalog, CatalogId> catalogRepository, CatalogId catalogId, CatalogCategoryId catalogCategoryId)
    {
        var catalogs = catalogRepository.AsQueryable();

        var query =
            from c in catalogs
            from c1 in c.Categories.Where(_ => _.Id == catalogCategoryId)
            where c.Id == catalogId
            select new
            {
                Catalog = c,
                CatalogCategory = c1
            };

        var result = await query.FirstOrDefaultAsync();

        return result != null;
    }
}
