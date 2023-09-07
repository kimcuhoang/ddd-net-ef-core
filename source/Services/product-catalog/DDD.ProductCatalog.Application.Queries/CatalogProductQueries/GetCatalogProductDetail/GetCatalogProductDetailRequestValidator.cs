using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.CatalogProductQueries.GetCatalogProductDetail;

public class GetCatalogProductDetailRequestValidator : AbstractValidator<GetCatalogProductDetailRequest>
{
    public GetCatalogProductDetailRequestValidator()
    {
        RuleFor(x => x.CatalogProductId)
            .NotNull()
            .NotEqual(CatalogProductId.Empty);
    }
}
