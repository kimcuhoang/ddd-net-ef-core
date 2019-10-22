using System;
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
    public class CommandHandler : AsyncRequestHandler<CreateCatalogCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly AbstractValidator<CreateCatalogCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory,
            AbstractValidator<CreateCatalogCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog>();
            this._validator = validator;
        }

        #region Overrides of AsyncRequestHandler<CreateCatalogCategoryCommand>

        protected override async Task Handle(CreateCatalogCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.CatalogId == request.CatalogId,
                x => x.Include(c => c.Categories));

            CatalogCategory parent = null;

            if (request.ParentCatalogCategoryId != null)
            {
                parent = catalog.Categories.SingleOrDefault(x => x.CatalogCategoryId == request.ParentCatalogCategoryId);
            }

            catalog.AddCategory(request.CategoryId, request.DisplayName, parent);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
