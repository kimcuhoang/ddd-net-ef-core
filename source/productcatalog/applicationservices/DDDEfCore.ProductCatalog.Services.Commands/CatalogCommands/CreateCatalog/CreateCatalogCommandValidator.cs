using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog
{
    public class CreateCatalogCommandValidator : AbstractValidator<CreateCatalogCommand>
    {
        public CreateCatalogCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();

            When(x => x.Categories.Any(), () =>
            {
                RuleForEach(x => x.Categories).ChildRules(category =>
                {
                    category.RuleFor(x => x.DisplayName)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .NotEmpty();

                    category.RuleFor(x => x.CategoryId)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .Must(x => x.IsNotEmpty)
                        .WithMessage(x => $"{nameof(x.CategoryId)} is empty or invalid.")
                        .Must(x => CategoryMustExist(repositoryFactory, x))
                        .WithMessage(x => $"Category#{x.CategoryId} could not be found.");
                });
            });
        }

        private bool CategoryMustExist(IRepositoryFactory repositoryFactory, CategoryId categoryId)
        {
            var repository = repositoryFactory.CreateRepository<Category>();
            var category = repository.FindOneAsync(x => x.CategoryId == categoryId).Result;
            return category != null;
        }
    }
}
