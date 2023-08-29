using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;

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
                        .MustAsync((categoryId, token) => CategoryMustExist(repositoryFactory, categoryId, token))
                        .WithMessage(x => $"Category#{x.CategoryId} could not be found.");
                });
            });
        }

        private async Task<bool> CategoryMustExist(IRepositoryFactory repositoryFactory, CategoryId categoryId, CancellationToken cancellationToken)
        {
            var repository = repositoryFactory.CreateRepository<Category, CategoryId>();
            var category = await repository.FindOneAsync(x => x.Id == categoryId);
            return category != null;
        }
    }
}
