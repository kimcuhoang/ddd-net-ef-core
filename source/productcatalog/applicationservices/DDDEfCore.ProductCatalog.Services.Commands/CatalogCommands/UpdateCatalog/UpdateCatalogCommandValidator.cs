using System.Data;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog
{
    public class UpdateCatalogCommandValidator : AbstractValidator<UpdateCatalogCommand>
    {
        public UpdateCatalogCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage(x => $"{nameof(x.CatalogId)} is empty or invalid");

            When(x => x.CatalogId != null && x.CatalogId.IsNotEmpty, () =>
            {
                RuleFor(command => command).Custom((command, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Catalog>();
                    var catalog = repository.FindOneAsync(x => x.CatalogId == command.CatalogId).Result;

                    if (catalog == null)
                    {
                        context.AddFailure(nameof(command.CatalogId),
                            $"Catalog#{command.CatalogId} could not be found.");
                    }
                });
            });

            RuleFor(x => x.CatalogName).Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().NotEmpty();
        }
    }
}
