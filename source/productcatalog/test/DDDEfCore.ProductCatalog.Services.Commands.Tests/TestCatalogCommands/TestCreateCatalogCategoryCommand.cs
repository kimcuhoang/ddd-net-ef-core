//using AutoFixture;
//using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
//using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
//using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
//using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;
//using FluentValidation;
//using FluentValidation.TestHelper;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Moq.EntityFrameworkCore;
//using Shouldly;
//using Xunit;

//namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands
//{
//    /// <summary>
//    /// https://github.com/MichalJankowskii/Moq.EntityFrameworkCore
//    /// </summary>
//    public class TestCreateCatalogCategoryCommand : UnitTestBase<Catalog, CatalogId>, IAsyncLifetime
//    {
//        private Mock<DbContext> _mockDbContext;
//        private CreateCatalogCategoryCommandValidator _validator;
//        private IRequestHandler<CreateCatalogCategoryCommand> _requestHandler;

//        private Catalog _catalog;
//        private Category _category;
        
//        #region Implementation of IAsyncLifetime

//        public Task InitializeAsync()
//        {
//            this._mockDbContext = new Mock<DbContext>();
//            var catalogRepository = new DefaultRepositoryAsync<Catalog, CatalogId>(this._mockDbContext.Object);
//            var categoryRepository = new DefaultRepositoryAsync<Category, CategoryId>(this._mockDbContext.Object);

//            this.MockRepositoryFactory.Setup(x => x.CreateRepository<Catalog, CatalogId>())
//                .Returns(catalogRepository);

//            this.MockRepositoryFactory.Setup(x => x.CreateRepository<Category, CategoryId>())
//                .Returns(categoryRepository);

//            this._catalog = Catalog.Create(this.Fixture.Create<string>());
//            this._category = Category.Create(this.Fixture.Create<string>());

//            this._mockDbContext.Setup(x => x.Set<Catalog>())
//                .ReturnsDbSet(new List<Catalog> { this._catalog });
//            this._mockDbContext.Setup(x => x.Set<Category>())
//                .ReturnsDbSet(new List<Category> { this._category });

//            this._validator = new CreateCatalogCategoryCommandValidator(catalogRepository, categoryRepository);

//            this._requestHandler = new CommandHandler(catalogRepository, this._validator);

//            return Task.CompletedTask;
//        }

//        public Task DisposeAsync() => Task.CompletedTask;

//        #endregion

//        [Fact(DisplayName = "Create CatalogCategory Successfully")]
//        public async Task Create_CatalogCategory_Successfully()
//        {
//            var command = new CreateCatalogCategoryCommand
//                {
//                    CatalogId = this._catalog.Id,
//                    CategoryId = this._category.Id,
//                    DisplayName = this.Fixture.Create<string>()
//                };

//            await this._requestHandler.Handle(command, this.CancellationToken);

//            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
//        }

//        [Fact(DisplayName = "Create CatalogCategory As Child Successfully")]
//        public async Task Create_CatalogCategory_As_Child_Successfully()
//        {
//            var catalogCategory = this._catalog.AddCategory(this._category.Id, this._category.DisplayName);
//            var childCategory = Category.Create(this.Fixture.Create<string>());

//            this._mockDbContext
//                .Setup(x => x.Set<Category>())
//                .ReturnsDbSet(new List<Category> { this._category, childCategory });

//            var command = new CreateCatalogCategoryCommand
//            {
//                CatalogId = this._catalog.Id,
//                CategoryId = childCategory.Id,
//                DisplayName = childCategory.DisplayName,
//                ParentCatalogCategoryId = catalogCategory.Id
//            };

//            await this._requestHandler.Handle(command, this.CancellationToken);

//            this._mockDbContext.Verify(x => x.Update(this._catalog), Times.Once);
//        }

//        [Fact(DisplayName = "Create CatalogCategory With Fail of Validation Should Throw Exception")]
//        public async Task Create_CatalogCategory_With_Fail_Of_Validation_ShouldThrowException()
//        {
//            var command = new CreateCatalogCategoryCommand
//            {
//                CatalogId = CatalogId.Empty,
//                CategoryId = CategoryId.Empty,
//                DisplayName = string.Empty
//            };

//            await Should.ThrowAsync<ValidationException>(async () => 
//                await this._requestHandler.Handle(command, this.CancellationToken));
//        }

//        [Fact(DisplayName = "Command With Empty Values Should Be Invalid")]
//        public async Task Command_With_Empty_Values_ShouldBeInvalid()
//        {
//            var command = new CreateCatalogCategoryCommand
//            {
//                CatalogId = CatalogId.Empty,
//                CategoryId = CategoryId.Empty,
//                DisplayName = string.Empty
//            };

//            var result = await this._validator.TestValidateAsync(command);

//            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
//            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
//            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
//        }

//        [Fact(DisplayName = "Command With Not Found Catalog Should Be Invalid")]
//        public async Task Command_With_NotFound_Catalog_ShouldBeInvalid()
//        {
//            var command = new CreateCatalogCategoryCommand
//            {
//                CatalogId = CatalogId.New,
//                CategoryId = this._category.Id,
//                DisplayName = this._category.DisplayName
//            };

//            var result = await this._validator.TestValidateAsync(command);

//            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
//            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
//            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
//        }

//        [Fact(DisplayName = "Command With Not Found Category Should Be Invalid")]
//        public async Task Command_With_NotFound_Category_ShouldBeInvalid()
//        {
//            var command = new CreateCatalogCategoryCommand
//            {
//                CatalogId = this._catalog.Id,
//                CategoryId = CategoryId.New,
//                DisplayName = this.Fixture.Create<string>()
//            };

//            var result = await this._validator.TestValidateAsync(command);

//            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
//            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
//            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
//        }

//        [Fact(DisplayName = "Command With Invalid ParentCatalogCategoryId Should Be Invalid")]
//        public async Task Command_With_Invalid_ParentCatalogCategoryId_ShouldBeInvalid()
//        {

//            var command = new CreateCatalogCategoryCommand
//            {
//                CatalogId = this._catalog.Id,
//                CategoryId = this._category.Id,
//                DisplayName = this.Fixture.Create<string>(),
//                ParentCatalogCategoryId = CatalogCategoryId.New
//            };

//            var result = await this._validator.TestValidateAsync(command);

//            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
//            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
//            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
//            result.ShouldHaveValidationErrorFor(x => x.ParentCatalogCategoryId);
//        }
//    }
//}
