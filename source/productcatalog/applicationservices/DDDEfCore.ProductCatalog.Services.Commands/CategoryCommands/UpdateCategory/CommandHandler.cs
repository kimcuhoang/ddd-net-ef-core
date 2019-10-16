using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.Exceptions;
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

        private readonly AbstractValidator<UpdateCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, AbstractValidator<UpdateCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Category>();
            this._validator = validator;
        }

        #region Overrides of AsyncRequestHandler<UpdateCategoryCommand>

        protected override async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            await this.ValidateCommand(request, cancellationToken);

            var categoryId = new CategoryId(request.CategoryId);
            var category = await this._repository.FindOneAsync(x => x.CategoryId == categoryId);
            
            if (category == null)
            {
                throw new NotFoundEntityException($"Category#{request.CategoryId}");
            }

            category.ChangeDisplayName(request.CategoryName);

            await this._repository.UpdateAsync(category);
        }

        #endregion

        private async Task ValidateCommand(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var validateResult = await this._validator.ValidateAsync(request, cancellationToken);
            if (!validateResult.IsValid)
            {
                throw new ValidationException($"Validation Failed For {nameof(UpdateCategoryCommand)}", validateResult.Errors);
            }
        }
    }
}
