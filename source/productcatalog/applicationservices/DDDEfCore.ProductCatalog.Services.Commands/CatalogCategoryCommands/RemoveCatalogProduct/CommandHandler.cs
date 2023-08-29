﻿using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct
{
    public class CommandHandler : IRequestHandler<RemoveCatalogProductCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog, CatalogId> _repository;
        private readonly IValidator<RemoveCatalogProductCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory,
            IValidator<RemoveCatalogProductCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = repositoryFactory.CreateRepository<Catalog, CatalogId>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task Handle(RemoveCatalogProductCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, cancellationToken);

            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.Id == request.CatalogId,
                x => x.Include(c => c.Categories).ThenInclude(c => c.Products));

            var catalogCategory =
                catalog.Categories.SingleOrDefault(x => x.Id == request.CatalogCategoryId);

            var catalogProduct =
                catalogCategory.Products.SingleOrDefault(x => x.Id == request.CatalogProductId);

            catalogCategory.RemoveCatalogProduct(catalogProduct);

            await this._repository.UpdateAsync(catalog);
        }
    }
}
