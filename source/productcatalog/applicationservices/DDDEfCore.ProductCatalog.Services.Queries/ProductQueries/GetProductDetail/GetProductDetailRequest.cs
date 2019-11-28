using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail
{
    public class GetProductDetailRequest : IRequest<GetProductDetailResult>
    {
        public ProductId ProductId { get; set; }
    }
}
