﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory
{
    public class CommandHandler : IRequestHandler<CreateCatalogCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog, CatalogId> _repository;
        private readonly IValidator<CreateCatalogCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory,
            IValidator<CreateCatalogCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog, CatalogId>();
            this._validator = validator;
        }

        #region Overrides of IRequestHandler<CreateCatalogCategoryCommand>

        public async Task Handle(CreateCatalogCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, cancellationToken);

            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.Id == request.CatalogId,
                x => x.Include(c => c.Categories));

            CatalogCategory parent = null;

            if (request.ParentCatalogCategoryId != null)
            {
                parent = catalog.Categories.SingleOrDefault(x => x.Id == request.ParentCatalogCategoryId);
            }

            catalog.AddCategory(request.CategoryId, request.DisplayName, parent);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
