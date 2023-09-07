using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;
using DNK.DDD.Core.Models;
using DDD.ProductCatalog.Core.Exceptions;

namespace DDD.ProductCatalog.Core.Catalogs;

public class CatalogCategory : EntityBase<CatalogCategoryId>
{
    public CatalogId CatalogId { get; private set; }

    public CategoryId CategoryId { get; private set; }

    public string DisplayName { get; private set; }

    public CatalogCategory? Parent { get; private set; }

    public bool IsRoot => this.Parent is null;

    private readonly List<CatalogProduct> _products = new();

    public IEnumerable<CatalogProduct> Products => this._products.AsReadOnly();

    private CatalogCategory(CatalogCategoryId id, string displayName, CatalogId catalogId, CategoryId categoryId) : base(id)
    {
        this.DisplayName = displayName;
        this.CatalogId = catalogId;
        this.CategoryId = categoryId;
    }

    #region Creations

    internal static CatalogCategory Create(CatalogId catalogId, CategoryId categoryId, string displayName, CatalogCategory? parent = null)
        => new(CatalogCategoryId.New, displayName, catalogId, categoryId)
        {
            //CatalogId = catalogId,
            //CategoryId = categoryId,
            //DisplayName = displayName,
            Parent = parent
        };

    #endregion

    #region Behaviors

    public CatalogCategory ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException($"{nameof(displayName)} is empty.");
        }

        this.DisplayName = displayName;

        return this;
    }

    #endregion

    #region Behaviors with CatalogProduct

    public CatalogProduct CreateCatalogProduct(ProductId productId, string displayName)
    {
        if (productId is null)
            throw new DomainException($"{nameof(productId)} is null.");

        if (this._products.Any(x => x.ProductId == productId))
            throw new DomainException($"Product#{productId} is existing in CatalogCategory#{this.Id}");

        var catalogProduct = CatalogProduct.Create(productId, displayName, this);

        this._products.Add(catalogProduct);

        return catalogProduct;
    }

    public void RemoveCatalogProduct(CatalogProduct catalogProduct)
    {
        if (catalogProduct is null)
            throw new DomainException($"{nameof(catalogProduct)} is null");

        this._products.RemoveAll(x => x == catalogProduct);
    }

    #endregion
}
