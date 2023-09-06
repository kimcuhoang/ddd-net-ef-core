using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection;

public class GetProductCollectionRequest : IRequest<GetProductCollectionResult>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
