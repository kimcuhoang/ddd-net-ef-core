using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog
{
    public class CommandHandler : IRequestHandler<UpdateCatalogCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog, CatalogId> _repository;
        private readonly IValidator<UpdateCatalogCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, IValidator<UpdateCatalogCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog, CatalogId>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of IRequestHandler<UpdateCatalogCommand>

        public async Task Handle(UpdateCatalogCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, cancellationToken);

            var catalog = await this._repository.FindOneAsync(x => x.Id == request.CatalogId);

            catalog.ChangeDisplayName(request.CatalogName);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
//TODO: Missing UnitTest