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

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogProductCommands.UpdateCatalogProduct
{
    public class CommandHandler : AsyncRequestHandler<UpdateCatalogProductCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly IValidator<UpdateCatalogProductCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, IValidator<UpdateCatalogProductCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<UpdateCatalogProductCommand>

        protected override async Task Handle(UpdateCatalogProductCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.CatalogId == request.CatalogId,
                x => x.Include(c => c.Categories).ThenInclude(c => c.Products));

            var catalogCategory =
                catalog.Categories.SingleOrDefault(x => x.CatalogCategoryId == request.CatalogCategoryId);

            var catalogProduct =
                catalogCategory.Products.SingleOrDefault(x => x.CatalogProductId == request.CatalogProductId);

            catalogProduct.ChangeDisplayName(request.DisplayName);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}

//TODO: Missing UnitTest