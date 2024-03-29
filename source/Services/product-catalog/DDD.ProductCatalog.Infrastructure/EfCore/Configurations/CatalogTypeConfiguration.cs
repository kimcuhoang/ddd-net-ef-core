﻿using DDD.ProductCatalog.Core.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Configurations;

public class CatalogTypeConfiguration : IEntityTypeConfiguration<Catalog>
{
    #region Implementation of IEntityTypeConfiguration<Catalog>

    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        builder.HasKey(_ => _.Id);

        builder
            .Property(x => x.Id)
            .HasConversion(x => x.Id, id => CatalogId.Of(id));

        builder
            .HasMany(x => x.Categories)
            .WithOne()
            .HasForeignKey(x => x.CatalogId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    #endregion
}
