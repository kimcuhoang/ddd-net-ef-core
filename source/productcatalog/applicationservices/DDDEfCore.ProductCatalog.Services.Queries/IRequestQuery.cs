using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries;
public interface IRequestQuery<TResponse>: IRequest<TResponse> where TResponse: notnull
{
}
