using MediatR;

namespace DNK.DDD.Application;

public interface IQueryRequest<TResponse> : IRequest<TResponse> where TResponse : notnull
{
}
