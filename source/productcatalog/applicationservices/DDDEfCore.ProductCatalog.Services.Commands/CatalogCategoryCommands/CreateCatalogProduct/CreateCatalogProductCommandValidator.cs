using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct
{
    public class CreateCatalogProductCommandValidator : AbstractValidator<CreateCatalogProductCommand>
    {
        public CreateCatalogProductCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage(x => $"{nameof(x.CatalogId)} is empty or invalid.");

            RuleFor(x => x.CatalogCategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage(x => $"{nameof(x.CatalogCategoryId)} is empty or invalid.");

            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage(x => $"{nameof(x.ProductId)} is empty or invalid.")
                .Must(x => this.ProductMustExist(repositoryFactory, x))
                .WithMessage(x => $"Product#{x.ProductId} could not be found.");

            When(this.CommandIsValid, () =>
            {
                RuleFor(command => command).Custom((command, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Catalog>();
                    var catalog = repository.FindOneWithIncludeAsync(x => x.CatalogId == command.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products)).GetAwaiter().GetResult();

                    if (catalog == null)
                    {
                        context.AddFailure($"{nameof(command.CatalogId)}", $"Catalog#{command.CatalogId} could not be found.");
                    }
                    else
                    {
                        var catalogCategory =
                            catalog.Categories.SingleOrDefault(x => x.CatalogCategoryId == command.CatalogCategoryId);

                        if (catalogCategory == null)
                        {
                            context.AddFailure($"{nameof(command.CatalogCategoryId)}", 
                                $"CatalogCategory#{command.CatalogCategoryId} could not be found in Catalog#{command.CatalogId}.");
                        }
                        else
                        {
                            if (catalogCategory.Products.Any(x => x.ProductId == command.ProductId))
                            {
                                context.AddFailure($"{nameof(command.ProductId)}",
                                    $"Product#{command.ProductId} is existing in CatalogCategory#{command.CatalogCategoryId}.");
                            }
                        }
                    }
                });
            });

            RuleFor(x => x.DisplayName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();
        }

        private bool CommandIsValid(CreateCatalogProductCommand command)
        {
            return command.CatalogId != null && command.CatalogId.IsNotEmpty 
                   && command.CatalogCategoryId != null && command.CatalogCategoryId.IsNotEmpty
                   && command.ProductId != null && command.ProductId.IsNotEmpty;
        }

        private bool ProductMustExist(IRepositoryFactory repositoryFactory, ProductId productId)
        {
            var repository = repositoryFactory.CreateRepository<Product>();
            var product = repository.FindOneAsync(x => x.ProductId == productId).Result;
            return product != null;
        }
    }
}
