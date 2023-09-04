using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;

public class CreateCatalogCategoryCommandValidator : AbstractValidator<CreateCatalogCategoryCommand>
{
    public CreateCatalogCategoryCommandValidator(IRepository<Catalog, CatalogId> catalogRepository,
                                                IRepository<Category, CategoryId> categoryRespository)
    {
        RuleFor(x => x.CatalogId)
            .NotNull()
            .MustAsync(async (x, token) => await this.CatalogIsExisting(catalogRepository, x))
            .WithMessage(x => $"Catalog#{x.CatalogId} could not be found.");

        RuleFor(x => x.CategoryId)
            .NotNull()
            .MustAsync(async (x, token) => await this.CategoryIsExisting(categoryRespository, x))
            .WithMessage(x => $"Category#{x.CategoryId} could not be found.");

        RuleFor(x => x.DisplayName)
            .NotNull()
            .NotEmpty();

        When(x => x.ParentCatalogCategoryId != null, () =>
        {
            RuleFor(x => x.ParentCatalogCategoryId).NotNull();

            When(x => x.ParentCatalogCategoryId != null, () =>
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
        });
    }

    private async Task<bool> CatalogIsExisting(IRepository<Catalog, CatalogId> catalogRepository, CatalogId catalogId)
    {
        var catalog = await catalogRepository.FindOneAsync(x => x.Id == catalogId);
        return catalog != null;
    }

    private async Task<bool> CategoryIsExisting(IRepository<Category, CategoryId> categoryRepository, CategoryId categoryId)
    {
        var category = await categoryRepository.FindOneAsync(x => x.Id == categoryId);
        return category != null;
    }

    private async Task<bool> CatalogCategoryIsExistingInCatalog(IRepository<Catalog, CatalogId> catalogRepository, CatalogId catalogId, CatalogCategoryId catalogCategoryId)
    {
        var catalog = await catalogRepository
                .FindOneWithIncludeAsync(x => x.Id == catalogId, x => x.Include(c => c.Categories));

        return catalog != null && catalog.Categories.Any(x => x.Id == catalogCategoryId);
    }
}
