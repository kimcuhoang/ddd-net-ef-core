using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands;

public interface ITransactionCommand<TResponse>: IRequest<TResponse> where TResponse : notnull
{
}
