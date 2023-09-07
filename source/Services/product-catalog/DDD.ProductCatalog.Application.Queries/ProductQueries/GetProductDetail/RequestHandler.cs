using Dapper;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;
using MediatR;
using System.Data;
using System.Text;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;

public class RequestHandler : IRequestHandler<GetProductDetailRequest, GetProductDetailResult>
{
    private readonly IDbConnection _dbConnection;

    public RequestHandler(IDbConnection dbConnection)
    {
        this._dbConnection = dbConnection;
    }


    #region Implementation of IRequestHandler<in GetProductDetailRequest,GetProductDetailResult>

    public async Task<GetProductDetailResult> Handle(GetProductDetailRequest request, CancellationToken cancellationToken)
    {

        var sqlClauses = new List<string>
        {
            this.SqlClauseForQueryingProduct(),
            this.SqlClauseForQueryingCatalogCategory()
        };
        var combinedSqlClauses = string.Join("; ", sqlClauses);
        var parameters = new { request.ProductId };

        var multiQueries = await this._dbConnection.QueryMultipleAsync(combinedSqlClauses, parameters);

        var product = await multiQueries.ReadFirstOrDefaultAsync<GetProductDetailResult.ProductDetailResult>();
        var catalogCategories = await multiQueries.ReadAsync<GetProductDetailResult.CatalogCategoryResult>();

        var result = new GetProductDetailResult
        {
            Product = product ?? new GetProductDetailResult.ProductDetailResult(),
            CatalogCategories = catalogCategories ?? Enumerable.Empty<GetProductDetailResult.CatalogCategoryResult>()
        };

        return result;
    }

    #endregion

    private string SqlClauseForQueryingProduct()
    {
        var fields = new Dictionary<string, string>
        {
            { nameof(GetProductDetailResult.Product.Id), $"{nameof(Product)}.Id" },
            { nameof(GetProductDetailResult.Product.Name), $"{nameof(Product)}.{nameof(Product.Name)}" }
        };

        var selectedFields = string.Join(", ", fields.Select(x => $"{x.Key}={x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(Product)} AS {nameof(Product)}")
            .Append($" WHERE {nameof(Product)}.Id = @ProductId");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForQueryingCatalogCategory()
    {
        var fields = new Dictionary<string, string>
        {
            { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogCategoryId), $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)}" },
            { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogCategoryName), $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.DisplayName)}" },
            { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogId), $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}" },
            { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogName), $"{nameof(Catalog)}.{nameof(Catalog.DisplayName)}" },
            { nameof(GetProductDetailResult.CatalogCategoryResult.ProductDisplayName), $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)}" },
            { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogProductId), $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Id)}" }
        };

        var selectedFields = string.Join(", ", fields.Select(x => $"{x.Key}={x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(CatalogProduct)} AS {nameof(CatalogProduct)}")
            .Append($" INNER JOIN {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
            .Append($" ON {nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)} = {nameof(CatalogProduct)}.{nameof(CatalogCategory)}Id")
            .Append($" INNER JOIN {nameof(Catalog)} AS {nameof(Catalog)}")
            .Append($" ON {nameof(Catalog)}.Id = {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}")
            .Append($" WHERE {nameof(CatalogProduct)}.{nameof(CatalogProduct.ProductId)} = @ProductId");

        return sqlClauseBuilder.ToString();
    }
}
