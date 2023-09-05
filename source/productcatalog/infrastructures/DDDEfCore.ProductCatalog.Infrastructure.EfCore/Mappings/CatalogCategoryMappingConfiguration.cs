﻿using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Mappings
{
    public class CatalogCategoryMappingConfiguration : IEntityTypeConfiguration<CatalogCategory>
    {
        #region Implementation of IEntityTypeConfiguration<CatalogCategory>

        public void Configure(EntityTypeBuilder<CatalogCategory> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .Property(x => x.Id)
                .HasConversion(x => x.Id, id => CatalogCategoryId.Of(id));

            builder
                .Property(x => x.CatalogId)
                .IsRequired()
                .HasConversion(x => x.Id, id => CatalogId.Of(id));

            builder
                .Property(x => x.CategoryId)
                .IsRequired()
                .HasConversion(x => x.Id, id => CategoryId.Of(id));

            builder
                .HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey("CatalogCategoryParentId")
                .IsRequired(false);

            builder.Ignore(x => x.IsRoot);
        }

        #endregion
    }
}
