using MediatR;

namespace DNK.DDD.Application;

public interface ITransactionCommand<TResponse> : IRequest<TResponse> where TResponse : notnull
{
}
