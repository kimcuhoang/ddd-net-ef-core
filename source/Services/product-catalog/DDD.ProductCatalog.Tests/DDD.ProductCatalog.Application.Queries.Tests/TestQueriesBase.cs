using DNK.DDD.IntegrationTests;
using FluentValidation.TestHelper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.ProductCatalog.Application.Queries.Tests;

[Collection(nameof(QueriesTestCollection))]
public abstract class TestQueriesBase(TestQueriesCollectionFixture testQueriesCollectionFixture, ITestOutputHelper output) : IntegrationTestBase<TestQueriesCollectionFixture, DefaultWebApplicationFactory, Program>(testQueriesCollectionFixture, output)
{
    public async Task ExecuteTestRequestHandler<TRequest, TResult>(TRequest request, Action<TResult> assert)
        where TRequest : IRequest<TResult>
    {
        await this.ExecuteServiceAsync(async serviceProvider =>
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

            var result = await handler.Handle(request, CancellationToken.None);

            assert(result);
        });
    }

    public async Task ExecuteValidationTest<TRequest>(TRequest request, Action<TestValidationResult<TRequest>> assert) where TRequest : class
    {

        await this.ExecuteServiceAsync(async serviceProvider =>
        {
            var validator = serviceProvider.GetRequiredService<IValidator<TRequest>>();

            var result = validator.TestValidate(request);

            assert(result);

            await Task.Yield();
        });
    }
}
