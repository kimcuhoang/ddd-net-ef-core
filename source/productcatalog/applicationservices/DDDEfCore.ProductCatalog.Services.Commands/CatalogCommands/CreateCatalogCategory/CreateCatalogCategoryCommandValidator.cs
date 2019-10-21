using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory
{
    public class CreateCatalogCategoryCommandValidator : AbstractValidator<CreateCatalogCategoryCommand>
    {
        public CreateCatalogCategoryCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage(x => $"{nameof(x.CatalogId)} is empty or invalid.")
                .Must(x => this.CatalogIsExisting(repositoryFactory, x))
                .WithMessage(x => $"Catalog#{x.CatalogId} could not be found.");

            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage(x => $"{nameof(x.CategoryId)} is empty or invalid.")
                .Must(x => this.CategoryIsExisting(repositoryFactory, x))
                .WithMessage(x => $"Category#{x.CategoryId} could not be found.");

            RuleFor(x => x.DisplayName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();

            When(x => x.ParentCatalogCategoryId != null, () =>
            {
                RuleFor(x => x.ParentCatalogCategoryId)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull()
                    .Must(x => x.IsNotEmpty)
                    .WithMessage(x => $"{nameof(x.ParentCatalogCategoryId)} is empty or invalid.");

                When(x => x.ParentCatalogCategoryId.IsNotEmpty, () =>
                {
                    RuleFor(x => x).Custom((x, context) =>
                    {
                        if (!this.CatalogCategoryIsExistingInCatalog(repositoryFactory, x.CatalogId,
                            x.ParentCatalogCategoryId))
                        {
                            context.AddFailure(nameof(CreateCatalogCategoryCommand.ParentCatalogCategoryId),
                                $"Could not found in Catalog#{x.CatalogId}");
                        }
                    });
                });
            });
        }

        private bool CatalogIsExisting(IRepositoryFactory repositoryFactory, CatalogId catalogId)
        {
            var repository = repositoryFactory.CreateRepository<Catalog>();
            var catalog = repository.FindOneAsync(x => x.CatalogId == catalogId).GetAwaiter().GetResult();
            return catalog != null;
        }

        private bool CategoryIsExisting(IRepositoryFactory repositoryFactory, CategoryId categoryId)
        {
            var repository = repositoryFactory.CreateRepository<Category>();
            var category = repository.FindOneAsync(x => x.CategoryId == categoryId).GetAwaiter().GetResult();
            return category != null;
        }

        private bool CatalogCategoryIsExistingInCatalog(IRepositoryFactory repositoryFactory, CatalogId catalogId, CatalogCategoryId catalogCategoryId)
        {
            var repository = repositoryFactory.CreateRepository<Catalog>();
            var catalog = repository.FindOneWithIncludeAsync(x => x.CatalogId == catalogId,
                x => x.Include(c => c.Categories)).GetAwaiter().GetResult();
            return catalog != null && catalog.Categories.Any(x => x.CatalogCategoryId == catalogCategoryId);
        }
    }
}
