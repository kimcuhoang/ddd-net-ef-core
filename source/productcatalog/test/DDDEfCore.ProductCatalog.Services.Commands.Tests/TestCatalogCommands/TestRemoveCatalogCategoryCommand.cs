using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands
{
    public class TestRemoveCatalogCategoryCommand : UnitTestBase<Catalog>
    {
        private readonly Mock<DbContext> _mockDbContext;
        private readonly IRepository<Catalog> _repository;
        private readonly RemoveCatalogCategoryCommandValidator _validator;

        public TestRemoveCatalogCategoryCommand() : base()
        {
            this._mockDbContext = new Mock<DbContext>();
            this._repository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            this._validator = new RemoveCatalogCategoryCommandValidator(this.MockRepositoryFactory.Object);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>())
                .Returns(this._repository);
        }

        [Fact(DisplayName = "Remove CatalogCategory Successfully")]
        public async Task Remove_CatalogCategory_Successfully()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> {catalog};
            this._mockDbContext.Setup(x => x.Set<Catalog>()).ReturnsDbSet(catalogs);

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this.Fixture.Create<string>());

            var command = new RemoveCatalogCategoryCommand(catalog.CatalogId.Id, catalogCategory.CatalogCategoryId.Id);

            IRequestHandler<RemoveCatalogCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(catalog), Times.Once);
        }
    }
}
