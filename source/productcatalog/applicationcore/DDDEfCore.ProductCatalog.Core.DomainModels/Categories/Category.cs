using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Categories
{
    public class Category : AggregateRoot<CategoryId>
    {
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

        private Category(string categoryName) : this(CategoryId.New, categoryName) { }

        private Category() { }

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
