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

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory
{
    public class CommandHandler : AsyncRequestHandler<UpdateCatalogCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly AbstractValidator<UpdateCatalogCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory,
                                AbstractValidator<UpdateCatalogCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<UpdateCatalogCategoryCommand>

        protected override async Task Handle(UpdateCatalogCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);
            
            var catalog = await this._repository.FindOneWithIncludeAsync(x => x.CatalogId == request.CatalogId,
                x => x.Include(c => c.Categories));

            catalog.Categories
                .SingleOrDefault(x => x.CatalogCategoryId == request.CatalogCategoryId)
                .ChangeDisplayName(request.DisplayName);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
