using DNK.DDD.Core.Models;

namespace DDD.ProductCatalog.Core.Catalogs;

public class CatalogCategoryId : IdentityBase
{
    #region Constructors

    private CatalogCategoryId(Guid id) : base(id) { }

    #endregion

    public static CatalogCategoryId New => new CatalogCategoryId(Guid.NewGuid());
    public static CatalogCategoryId Of(Guid id) => new CatalogCategoryId(id);
    public static CatalogCategoryId Empty => new CatalogCategoryId(Guid.Empty);
}
