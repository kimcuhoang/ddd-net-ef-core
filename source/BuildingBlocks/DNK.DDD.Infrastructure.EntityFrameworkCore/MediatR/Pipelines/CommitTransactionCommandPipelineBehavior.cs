using DNK.DDD.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore.MediatR.Pipelines;

public class CommitTransactionCommandPipelineBehavior<TRequest, TResponse>(DbContext dbContext) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionCommand<TResponse>
    where TResponse: notnull
{
    private readonly DbContext _dbContext = dbContext;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        var strategy = this._dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            // Achieving atomicity
            await using var transaction = await this._dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await this._dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return response;
    }
}
