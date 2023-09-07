using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Configurations;

public class CatalogProductTypeConfiguration : IEntityTypeConfiguration<CatalogProduct>
{
    #region Implementation of IEntityTypeConfiguration<CatalogProduct>

    public void Configure(EntityTypeBuilder<CatalogProduct> builder)
    {
        builder.HasKey(_ => _.Id);

        builder
            .Property(x => x.Id)
            .HasConversion(x => x.Id, id => CatalogProductId.Of(id));

        builder
            .Property(x => x.ProductId)
            .IsRequired()
            .HasConversion(x => x.Id, id => ProductId.Of(id));

        builder
            .HasOne(x => x.CatalogCategory)
            .WithMany(x => x.Products)
            .HasForeignKey("CatalogCategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    #endregion
}
