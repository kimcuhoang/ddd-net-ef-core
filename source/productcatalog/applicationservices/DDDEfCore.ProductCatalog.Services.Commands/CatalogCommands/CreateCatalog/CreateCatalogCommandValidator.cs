﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog
{
    public class CreateCatalogCommandValidator : AbstractValidator<CreateCatalogCommand>
    {
        public CreateCatalogCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogName)
                
                .NotNull()
                .NotEmpty();

            When(x => x.Categories.Any(), () =>
            {
                RuleForEach(x => x.Categories).ChildRules(category =>
                {
                    category.RuleFor(x => x.DisplayName)
                        
                        .NotNull()
                        .NotEmpty();

                    category.RuleFor(x => x.CategoryId)
                        
                        .NotNull().NotEqual(CategoryId.Empty)
                        .Must(x => CategoryMustExist(repositoryFactory, x))
                        .WithMessage(x => $"Category#{x.CategoryId} could not be found.");
                });
            });
        }

        private bool CategoryMustExist(IRepositoryFactory repositoryFactory, CategoryId categoryId)
        {
            var repository = repositoryFactory.CreateRepository<Category, CategoryId>();
            var category = repository.FindOneAsync(x => x.Id == categoryId).Result;
            return category != null;
        }
    }
}
