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
            builder
                .Property(x => x.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(x => x.Id, id => (CatalogProductId)id);

            builder
                .Property(x => x.ProductId)
                .IsRequired()
                .HasConversion(x => x.Id, id => (ProductId)id);

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
