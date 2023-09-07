using AutoFixture.Xunit2;
using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Application.Commands.CatalogCommands.UpdateCatalog;
using FakeItEasy;
using FluentValidation.TestHelper;

namespace DDD.ProductCatalog.Application.Commands.Tests.TestCatalogCommands;
public class TestUpdateCatalogCommand
{
    private readonly IRepository<Catalog, CatalogId> _catalogRepository;

    public TestUpdateCatalogCommand()
    {
        this._catalogRepository = A.Fake<IRepository<Catalog, CatalogId>>();
    }

    [Theory]
    [AutoData]
    public async Task UpdateCatalogSuccessfully(string initialName, string modifiedName)
    {
        var catalog = Catalog.Create(initialName);

        A.CallTo(() => this._catalogRepository.FindOneAsync(default!))
            .WithAnyArguments()
            .Returns(Task.FromResult((Catalog?)catalog));

        var commandHandler = new CommandHandler(this._catalogRepository);

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

        var validator = new UpdateCatalogCommandValidator(this._catalogRepository);

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

        var validator = new UpdateCatalogCommandValidator(this._catalogRepository);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(_ => _.CatalogId);
        result.ShouldNotHaveValidationErrorFor(_ => _.CatalogName);
    }

    [Fact]
    public async Task EmptyCatalogNameShouldNotPassValidator()
    {
        var catalog = Catalog.Create("Catalog");

        A.CallTo(() => this._catalogRepository.FindOneAsync(default!))
            .WithAnyArguments()
            .Returns(Task.FromResult((Catalog?)catalog));

        var command = new UpdateCatalogCommand
        {
            CatalogId = catalog.Id,
            CatalogName = string.Empty
        };

        var validator = new UpdateCatalogCommandValidator(this._catalogRepository);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(_ => _.CatalogId);
        result.ShouldHaveValidationErrorFor(_ => _.CatalogName);
    }
}
