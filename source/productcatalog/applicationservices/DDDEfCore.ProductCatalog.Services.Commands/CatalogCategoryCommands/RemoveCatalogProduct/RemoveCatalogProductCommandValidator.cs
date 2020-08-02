using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct
{
    public class RemoveCatalogProductCommandValidator : AbstractValidator<RemoveCatalogProductCommand>
    {
        public RemoveCatalogProductCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull();

            RuleFor(x => x.CatalogCategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull();

            RuleFor(x => x.CatalogProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull();

            When(CommandIsValid, () =>
            {
                RuleFor(command => command).Custom((command, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Catalog, CatalogId>();
                    var catalog = repository.FindOneWithIncludeAsync(x => x.Id == command.CatalogId,
                        x => x.Include(c => c.Categories).ThenInclude(c => c.Products)).Result;

                    if (catalog == null)
                    {
                        context.AddFailure(nameof(command.CatalogId), $"Catalog#{command.CatalogId} could not be found.");
                    }
                    else
                    {
                        var catalogCategory =
                            catalog.Categories.SingleOrDefault(x => x.Id == command.CatalogCategoryId);
                        
                        if (catalogCategory == null)
                        {
                            context.AddFailure(nameof(command.CatalogCategoryId), 
                                $"CatalogCategory#{command.CatalogCategoryId} could not be found in Catalog#{command.CatalogId}");
                        }
                        else
                        {
                            if (catalogCategory.Products.All(x => x.Id != command.CatalogProductId))
                            {
                                context.AddFailure(nameof(command.CatalogProductId),
                                    $"CatalogProduct#{command.CatalogProductId} could not be found in CatalogCategory#{command.CatalogCategoryId} of Catalog#{command.CatalogId}");
                            }
                        }
                    }
                });
            });
        }

        private bool CommandIsValid(RemoveCatalogProductCommand command)
        {
            return command.CatalogId != null && command.CatalogCategoryId != null && command.CatalogProductId != null;
        }
    }
}
