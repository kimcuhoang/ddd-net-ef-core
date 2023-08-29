﻿using DDDEfCore.Core.Common;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.Infrastructure.PipelineBehaviors;

public class EndRequestPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
{
    private readonly IRepositoryFactory _repositoryFactory;

    public EndRequestPipelineBehavior(IRepositoryFactory repositoryFactory)
        => this._repositoryFactory = repositoryFactory;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (this._repositoryFactory)
        {
            var response = await next();
            return response;
        }
    }
}