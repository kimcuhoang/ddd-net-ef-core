using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries;

public class TestGetCategoryFixture : DefaultTestFixture
{
    public TestGetCategoryFixture(DefaultWebApplicationFactory factory) : base(factory)
    {
    }

    public Category Category { get; private set; } = default!;
    public Catalog Catalog { get; private set; } = default!;
    public CatalogCategory CatalogCategory { get; private set; } = default!;

    #region Overrides of SharedFixture

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this.Fixture.Create<string>());
        await this.SeedingData<Category, CategoryId>(this.Category);

        this.Catalog = Catalog.Create(this.Fixture.Create<string>());
        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        await this.SeedingData<Catalog,CatalogId>(this.Catalog);
    }

    #endregion
}
