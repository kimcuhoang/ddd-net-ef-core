using DDDEfCore.ProductCatalog.Services.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.MediatR.Pipelines;
public class CommandTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionCommand<TResponse>
    where TResponse: notnull
{
    private readonly DbContext _dbContext;

    public CommandTransactionPipelineBehavior(DbContext dbContext)
    {
        this._dbContext = dbContext;
    }

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
