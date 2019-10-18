using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory
{
    public class CommandHandler : AsyncRequestHandler<RemoveCatalogCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly AbstractValidator<RemoveCatalogCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory,
            AbstractValidator<RemoveCatalogCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<RemoveCatalogCategoryCommand>

        protected override async Task Handle(RemoveCatalogCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.CatalogId == request.CatalogId,
                x => x.Include(c => c.Categories));

            var catalogCategory =
                catalog.Categories.SingleOrDefault(x => x.CatalogCategoryId == request.CatalogCategoryId);

            catalog.RemoveCatalogCategoryWithDescendants(catalogCategory);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
