using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public class CommandHandler : AsyncRequestHandler<UpdateCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private readonly IRepository<Category> _repository;

        private readonly IValidator<UpdateCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, IValidator<UpdateCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Category>();
            this._validator = validator;
        }

        #region Overrides of AsyncRequestHandler<UpdateCategoryCommand>

        protected override async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var category = await this._repository.FindOneAsync(x => x.CategoryId == request.CategoryId);
            
            category.ChangeDisplayName(request.CategoryName);

            await this._repository.UpdateAsync(category);
        }

        #endregion
    }
}
