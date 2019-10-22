using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory
{
    public class UpdateCatalogCategoryCommandValidator : AbstractValidator<UpdateCatalogCategoryCommand>
    {
        public UpdateCatalogCategoryCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage($"{nameof(UpdateCatalogCategoryCommand.CatalogId)} is empty or invalid");

            RuleFor(x => x.CatalogCategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage($"{nameof(UpdateCatalogCategoryCommand.CatalogCategoryId)} is empty or invalid");

            When(x => x.CatalogId.IsNotEmpty && x.CatalogCategoryId.IsNotEmpty, () =>
            {
                RuleFor(x => x).Custom((x, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Catalog>();

                    var catalog = repository.FindOneWithIncludeAsync(c => c.CatalogId == x.CatalogId,
                        y => y.Include(c => c.Categories)).GetAwaiter().GetResult();

                    if (catalog == null)
                    {
                        context.AddFailure(nameof(x.CatalogId), $"Catalog#{x.CatalogId} could not be found");
                    }
                    else
                    {
                        if (catalog.Categories.All(c => c.CatalogCategoryId != x.CatalogCategoryId))
                        {
                            context.AddFailure(nameof(x.CatalogCategoryId),
                                $"CatalogCategory#{x.CatalogCategoryId} could not be found in Catalog#{x.CatalogId}");
                        }
                    }
                });
            });

            RuleFor(x => x.DisplayName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();
        }
    }
}
