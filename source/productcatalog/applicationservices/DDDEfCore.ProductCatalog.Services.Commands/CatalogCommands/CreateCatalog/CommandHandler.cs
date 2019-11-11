using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog
{
    public class CommandHandler : AsyncRequestHandler<CreateCatalogCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly IValidator<CreateCatalogCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, IValidator<CreateCatalogCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this._repository = this._repositoryFactory.CreateRepository<Catalog>();
        }

        #region Overrides of AsyncRequestHandler<CreateCatalogCommand>

        protected override async Task Handle(CreateCatalogCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var catalog = Catalog.Create(request.CatalogName);

            foreach (var category in request.Categories)
            {
                catalog.AddCategory(category.CategoryId, category.DisplayName);
            }

            await this._repository.AddAsync(catalog);

        }

        #endregion
    }
}
