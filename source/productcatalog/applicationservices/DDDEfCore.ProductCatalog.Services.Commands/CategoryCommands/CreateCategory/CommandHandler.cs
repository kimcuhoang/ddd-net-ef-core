using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory
{
    public class CommandHandler : AsyncRequestHandler<CreateCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private readonly IRepository<Category, CategoryId> _repository;

        private readonly IValidator<CreateCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, IValidator<CreateCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Category, CategoryId>();
            this._validator = validator;
        }

        #region Overrides of AsyncRequestHandler<CreateCategoryCommand>

        protected override async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var category = Category.Create(request.CategoryName);

            await this._repository.AddAsync(category);
        }

        #endregion
    }
}
