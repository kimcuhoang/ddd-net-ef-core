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

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct
{
    public class CommandHandler : AsyncRequestHandler<RemoveCatalogProductCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly AbstractValidator<RemoveCatalogProductCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory,
            AbstractValidator<RemoveCatalogProductCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = repositoryFactory.CreateRepository<Catalog>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<RemoveCatalogProductCommand>

        protected override async Task Handle(RemoveCatalogProductCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.CatalogId == request.CatalogId,
                x => x.Include(c => c.Categories).ThenInclude(c => c.Products));

            var catalogCategory =
                catalog.Categories.SingleOrDefault(x => x.CatalogCategoryId == request.CatalogCategoryId);

            var catalogProduct =
                catalogCategory.Products.SingleOrDefault(x => x.CatalogProductId == request.CatalogProductId);

            catalogCategory.RemoveCatalogProduct(catalogProduct);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
