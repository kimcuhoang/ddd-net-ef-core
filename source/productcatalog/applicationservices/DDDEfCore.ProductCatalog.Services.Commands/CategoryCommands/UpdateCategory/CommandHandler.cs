﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public class CommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private readonly IRepository<Category, CategoryId> _repository;

        private readonly IValidator<UpdateCategoryCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, IValidator<UpdateCategoryCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Category, CategoryId>();
            this._validator = validator;
        }

        #region Overrides of IRequestHandler<UpdateCategoryCommand>

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, cancellationToken);

            var category = await this._repository.FindOneAsync(x => x.Id == request.CategoryId);
            
            category.ChangeDisplayName(request.CategoryName);

            await this._repository.UpdateAsync(category);
        }

        #endregion
    }
}
