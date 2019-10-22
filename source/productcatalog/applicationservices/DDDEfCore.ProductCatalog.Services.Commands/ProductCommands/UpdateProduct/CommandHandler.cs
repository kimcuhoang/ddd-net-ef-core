using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct
{
    public class CommandHandler : AsyncRequestHandler<UpdateProductCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Product> _repository;
        private readonly AbstractValidator<UpdateProductCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, AbstractValidator<UpdateProductCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Product>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<UpdateProductCommand>

        protected override async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);
            
            var product = await this._repository.FindOneAsync(x => x.ProductId == request.ProductId);
            product.ChangeName(request.ProductName);

            await this._repository.UpdateAsync(product);
        }

        #endregion
    }
}
//TODO: Missing UnitTest