using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Xunit;
using AutoFixture;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries
{
    [Collection(nameof(Tests.SharedFixture))]
    public class TestGetCatalogCollectionFixture : BaseTestFixture<Catalog>, IAsyncLifetime
    {
        public TestGetCatalogCollectionFixture(SharedFixture sharedFixture) : base(sharedFixture) { }

        public List<Catalog> Catalogs { get; private set; } = new List<Catalog>();

        #region Implementation of IAsyncLifetime

        public async Task InitializeAsync()
        {
            var numberOfCatalogs = GenFu.GenFu.Random.Next(10);
            Enumerable.Range(0, numberOfCatalogs).ToList().ForEach(i =>
            {
                var catalog = this.CreateCatalog(GenFu.GenFu.Random.Next(5));
                this.Catalogs.Add(catalog);
            });
            await this.SharedFixture.SeedingData(this.Catalogs.ToArray());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        private Catalog CreateCatalog(int numberOfCategories)
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            Enumerable.Range(0, numberOfCategories).ToList().ForEach(i =>
            {
                var categoryId = IdentityFactory.Create<CategoryId>();
                catalog.AddCategory(categoryId, this.Fixture.Create<string>());
            });

            return catalog;
        }
    }
}
