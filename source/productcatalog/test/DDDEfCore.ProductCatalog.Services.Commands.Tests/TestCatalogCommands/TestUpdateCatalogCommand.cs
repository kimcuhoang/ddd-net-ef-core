using AutoFixture.Xunit2;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands;
public class TestUpdateCatalogCommand
{
    private readonly Mock<IRepository<Catalog, CatalogId>> _mockCatalogRepository;

    public TestUpdateCatalogCommand()
    {
        this._mockCatalogRepository = new Mock<IRepository<Catalog, CatalogId>>();
    }

    [Theory]
    [AutoData]
    public async Task UpdateCatalogSuccessfully(string initialName, string modifiedName)
    {
        var catalog = Catalog.Create(initialName);

        this._mockCatalogRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Catalog, bool>>>()))
            .ReturnsAsync(catalog);

        var commandHandler = new CommandHandler(this._mockCatalogRepository.Object);

        var command = new UpdateCatalogCommand
        {
            CatalogId = catalog.Id,
            CatalogName = modifiedName
        };

        var result = await commandHandler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CatalogId.ShouldBe(catalog.Id);
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task CatalogNotFoundShouldNotPassValidator()
    {
        var command = new UpdateCatalogCommand
        {
            CatalogId = CatalogId.New,
            CatalogName = "Catalog"
        };

        var validator = new UpdateCatalogCommandValidator(this._mockCatalogRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(_ => _.CatalogId).WithErrorMessage($"Catalog#{command.CatalogId} could not be found.");
        result.ShouldNotHaveValidationErrorFor(_ => _.CatalogName);
    }

    [Fact]
    public async Task InvalidCatalogIdShouldNotPassValidator()
    {
        var command = new UpdateCatalogCommand
        {
            CatalogId = CatalogId.Empty,
            CatalogName = "Catalog"
        };

        var validator = new UpdateCatalogCommandValidator(this._mockCatalogRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(_ => _.CatalogId);
        result.ShouldNotHaveValidationErrorFor(_ => _.CatalogName);
    }

    [Fact]
    public async Task EmptyCatalogNameShouldNotPassValidator()
    {
        var catalog = Catalog.Create("Catalog");

        this._mockCatalogRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Catalog, bool>>>()))
            .ReturnsAsync(catalog);

        var command = new UpdateCatalogCommand
        {
            CatalogId = catalog.Id,
            CatalogName = string.Empty
        };

        var validator = new UpdateCatalogCommandValidator(this._mockCatalogRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(_ => _.CatalogId);
        result.ShouldHaveValidationErrorFor(_ => _.CatalogName);
    }
}
