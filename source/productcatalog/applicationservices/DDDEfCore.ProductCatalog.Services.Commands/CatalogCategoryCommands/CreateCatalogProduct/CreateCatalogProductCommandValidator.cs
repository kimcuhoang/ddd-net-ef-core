using System;
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
                
                .NotNull();

            RuleFor(x => x.CatalogCategoryId)
                
                .NotNull();

            RuleFor(x => x.ProductId)
                
                .NotNull()
                .Must(x => this.ProductMustExist(repositoryFactory, x))
                .WithMessage(x => $"Product#{x.ProductId} could not be found.");

            When(this.CommandIsValid, () =>
            {
                RuleFor(command => command).Custom((command, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Catalog, CatalogId>();
                    var catalog = repository.FindOneWithIncludeAsync(x => x.Id == command.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products)).GetAwaiter().GetResult();

                    if (catalog == null)
                    {
                        context.AddFailure($"{nameof(command.CatalogId)}", $"Catalog#{command.CatalogId} could not be found.");
                    }
                    else
                    {
                        var catalogCategory =
                            catalog.Categories.SingleOrDefault(x => x.Id == command.CatalogCategoryId);

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
                
                .NotNull()
                .NotEmpty();
        }

        private bool CommandIsValid(CreateCatalogProductCommand command)
        {
            return command.CatalogId != null && command.CatalogCategoryId != null && command.ProductId != null;
        }

        private bool ProductMustExist(IRepositoryFactory repositoryFactory, ProductId productId)
        {
            var repository = repositoryFactory.CreateRepository<Product, ProductId>();
            var product = repository.FindOneAsync(x => x.Id == productId).Result;
            return product != null;
        }
    }
}
