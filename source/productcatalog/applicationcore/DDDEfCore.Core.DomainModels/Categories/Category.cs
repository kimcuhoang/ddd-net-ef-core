using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Core.DomainModels.Exceptions;

namespace DDDEfCore.Core.DomainModels.Categories
{
    public class Category : AggregateRoot
    {
        public CategoryId CategoryId => (CategoryId)this.Id;

        public string DisplayName { get; private set; }

        #region Constructors

        private Category(CategoryId categoryId, string categoryName) : base(categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new DomainException($"{nameof(categoryName)} is empty.");
            }

            this.DisplayName = categoryName;
        }

        private Category(string categoryName) : this(IdentityFactory.Create<CategoryId>(), categoryName) { }

        #endregion

        #region Creations

        public static Category Create(string categoryName) => new Category(categoryName);

        #endregion

        #region Behaviors

        public Category ChangeDisplayName(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new DomainException($"{nameof(categoryName)} is empty.");
            }

            this.DisplayName = categoryName;
            return this;
        }

        #endregion
    }
}
