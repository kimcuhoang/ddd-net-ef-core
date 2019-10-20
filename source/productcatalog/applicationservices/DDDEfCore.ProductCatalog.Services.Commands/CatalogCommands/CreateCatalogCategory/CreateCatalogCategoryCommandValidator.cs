using System;
using System.Linq;
using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory
{
    public class CreateCatalogCategoryCommandValidator : AbstractValidator<CreateCatalogCategoryCommand>
    {
        public CreateCatalogCategoryCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual(Guid.Empty)
                .Must(x => this.CatalogIsExisting(repositoryFactory, x));

            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual(Guid.Empty)
                .Must(x => this.CategoryIsExisting(repositoryFactory, x));

            RuleFor(x => x.DisplayName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();

            When(x => x.ParentCatalogCategoryId.HasValue, () =>
            {
                RuleFor(x => x).Custom((command, context) =>
                {
                    var parentCatalogCategoryId = command.ParentCatalogCategoryId.Value;
                    if (parentCatalogCategoryId != Guid.Empty && 
                        !this.CatalogCategoryIsExistingInCatalog(repositoryFactory, command.CatalogId, parentCatalogCategoryId))
                    {
                        context.AddFailure(nameof(CreateCatalogCategoryCommand.ParentCatalogCategoryId), $"Could not found in Catalog#{command.CatalogId}");
                    }
                });
            });
        }

        private bool CatalogIsExisting(IRepositoryFactory repositoryFactory, Guid catalogIdGuid)
        {
            var catalogId = new CatalogId(catalogIdGuid);
            var repository = repositoryFactory.CreateRepository<Catalog>();
            var catalog = repository.FindOneAsync(x => x.CatalogId == catalogId).GetAwaiter().GetResult();
            return catalog != null;
        }

        private bool CategoryIsExisting(IRepositoryFactory repositoryFactory, Guid categoryIdGuid)
        {
            var categoryId = new CategoryId(categoryIdGuid);
            var repository = repositoryFactory.CreateRepository<Category>();
            var category = repository.FindOneAsync(x => x.CategoryId == categoryId).GetAwaiter().GetResult();
            return category != null;
        }

        private bool CatalogCategoryIsExistingInCatalog(IRepositoryFactory repositoryFactory, Guid catalogIdGuid,
            Guid catalogCategoryIdGuid)
        {
            var catalogId = new CatalogId(catalogIdGuid);
            var catalogCategoryId = new CatalogCategoryId(catalogCategoryIdGuid);
            var repository = repositoryFactory.CreateRepository<Catalog>();
            var catalog = repository.FindOneWithIncludeAsync(x => x.CatalogId == catalogId,
                x => x.Include(c => c.Categories)).GetAwaiter().GetResult();
            return catalog != null && catalog.Categories.Any(x => x.CatalogCategoryId == catalogCategoryId);
        }
    }
}
