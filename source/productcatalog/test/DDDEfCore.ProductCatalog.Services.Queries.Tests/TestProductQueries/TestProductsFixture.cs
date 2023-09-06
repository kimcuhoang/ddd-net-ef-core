﻿using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestProductQueries;

public class TestProductsFixture : DefaultTestFixture
{
    public TestProductsFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Product Product { get; private set; } = default!;
    public Category Category { get; private set; } = default!;
    public Catalog Catalog { get; private set; } = default!;
    public CatalogCategory CatalogCategory { get; private set; } = default!;
    public CatalogProduct CatalogProduct { get; private set; } = default!;

    #region Overrides of SharedFixture

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Product = Product.Create(this.Fixture.Create<string>());
        await this.SeedingData<Product,ProductId>(this.Product);

        this.Category = Category.Create(this.Fixture.Create<string>());
        await this.SeedingData<Category,CategoryId>(this.Category);

        this.Catalog = Catalog.Create(this.Fixture.Create<string>());
        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);
        await this.SeedingData<Catalog,CatalogId>(this.Catalog);
    }

    #endregion
}
