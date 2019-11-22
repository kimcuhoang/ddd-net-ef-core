using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestProductQueries
{
    public abstract class TestProductQueriesBase
    {
        protected readonly TestProductsFixture _testProductsFixture;
        protected readonly CancellationToken _cancellationToken;

        protected TestProductQueriesBase(TestProductsFixture testProductsFixture)
        {
            this._testProductsFixture = testProductsFixture;
            this._cancellationToken = new CancellationToken(false);
        }

        protected async Task ExecuteTest<TRequest, TResult>(TRequest request, Action<TResult> assert) where TRequest : IRequest<TResult>
        {
            await this._testProductsFixture.ExecuteTest(async serviceProvider =>
            {
                var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

                var result = await handler.Handle(request, this._cancellationToken);

                assert(result);
            });
        }
    }
}
