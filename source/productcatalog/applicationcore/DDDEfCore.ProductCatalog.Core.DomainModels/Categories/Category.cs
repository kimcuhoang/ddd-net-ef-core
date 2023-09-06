﻿using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

public class Category : AggregateRoot<CategoryId>
{
    public string DisplayName { get; private set; }

    #region Constructors

    private Category(CategoryId id, string displayName) : base(id)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException($"{nameof(displayName)} is empty.");
        }

        this.DisplayName = displayName;
    }

    #endregion

    #region Creations

    public static Category Create(string categoryName) => new(CategoryId.New, categoryName);

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
