using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Mappings
{
    public class CatalogProductMappingConfiguration : IEntityTypeConfiguration<CatalogProduct>
    {
        #region Implementation of IEntityTypeConfiguration<CatalogProduct>

        public void Configure(EntityTypeBuilder<CatalogProduct> builder)
        {
            builder.HasKey(x => x.CatalogProductId);

            builder
                .Property(x => x.CatalogProductId)
                .HasField("Id")
                .HasColumnName("Id")
                .HasConversion(x => x.Id, x => new CatalogProductId(x));

            builder
                .Property(x => x.ProductId)
                .IsRequired()
                .HasConversion(x => x.Id, x => new ProductId(x));

            builder
                .HasOne(x => x.CatalogCategory)
                .WithMany(x => x.Products)
                .HasForeignKey("CatalogCategoryId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion
    }
}
