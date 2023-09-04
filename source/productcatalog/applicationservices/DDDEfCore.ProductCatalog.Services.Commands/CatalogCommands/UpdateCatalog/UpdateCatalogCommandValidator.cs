using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;

public class UpdateCatalogCommandValidator : AbstractValidator<UpdateCatalogCommand>
{
    public UpdateCatalogCommandValidator(IRepository<Catalog, CatalogId> catalogRepository)
    {
        RuleFor(x => x.CatalogId).NotNull();

        When(x => x.CatalogId != null, () =>
        {
            RuleFor(command => command).CustomAsync(async (command, context, token) =>
            {
                var catalog = await catalogRepository.FindOneAsync(x => x.Id == command.CatalogId);

                if (catalog == null)
                {
                    context.AddFailure(nameof(command.CatalogId), $"Catalog#{command.CatalogId} could not be found.");
                }
            });
        });

        RuleFor(x => x.CatalogName).NotNull().NotEmpty();
    }
}
