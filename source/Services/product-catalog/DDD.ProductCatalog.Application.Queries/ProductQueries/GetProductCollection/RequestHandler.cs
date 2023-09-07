using Dapper;
using DDD.ProductCatalog.Core.Products;
using MediatR;
using System.Data;
using System.Text;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;

public class RequestHandler : IRequestHandler<GetProductCollectionRequest, GetProductCollectionResult>
{
    private readonly IDbConnection _dbConnection;

    public RequestHandler(IDbConnection dbConnection)
    {
        this._dbConnection = dbConnection;
    }


    #region Implementation of IRequestHandler<in GetProductCollectionRequest,GetProductCollectionResult>

    public async Task<GetProductCollectionResult> Handle(GetProductCollectionRequest request, CancellationToken cancellationToken)
    {

        var sqlClauses = new List<string>
        {
            this.SqlClauseForQueryingProducts(request),
            this.SqlClauseForCountProducts(request)
        };

        var combinedSqlClauses = string.Join("; ", sqlClauses);
        var parameters = new
        {
            Offset = (request.PageIndex - 1) * request.PageSize,
            request.PageSize,
            SearchTerm = $"%{request.SearchTerm}%"
        };


        var multiQueries = await this._dbConnection.QueryMultipleAsync(combinedSqlClauses, parameters);

        var products = await multiQueries.ReadAsync<GetProductCollectionResult.ProductCollectionItem>();
        var totalProducts = await multiQueries.ReadFirstOrDefaultAsync<int>();

        var result = new GetProductCollectionResult
        {
            Products = products ?? Enumerable.Empty<GetProductCollectionResult.ProductCollectionItem>(),
            TotalProducts = totalProducts
        };

        return result;
    }

    #endregion

    private string SqlClauseForQueryingProducts(GetProductCollectionRequest request)
    {
        var fields = new Dictionary<string, string>
        {
            { nameof(GetProductCollectionResult.ProductCollectionItem.Id), $"{nameof(Product)}.Id" },
            { nameof(GetProductCollectionResult.ProductCollectionItem.DisplayName), $"{nameof(Product)}.{nameof(Product.Name)}" }
        };

        var groupByFields = string.Join(",", fields.Select(x => x.Value));
        var selectedFields = string.Join(", ", fields.Select(x => $"{x.Key}={x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(Product)} AS {nameof(Product)}");

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            sqlClauseBuilder = sqlClauseBuilder
                .Append($" WHERE {nameof(Product)}.{nameof(Product.Name)} LIKE @SearchTerm");
        }

        sqlClauseBuilder = sqlClauseBuilder
            .Append($" GROUP BY {groupByFields}")
            .Append($" ORDER BY {nameof(Product)}.{nameof(Product.Name)} ")
            .Append(" OFFSET @Offset ROWS ")
            .Append(" FETCH NEXT @PageSize ROWS ONLY; ");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForCountProducts(GetProductCollectionRequest request)
    {
        var sqlClauseBuilder = new StringBuilder($"SELECT COUNT(*)")
            .Append($" FROM {nameof(Product)}");

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            sqlClauseBuilder = sqlClauseBuilder
                .Append($" WHERE {nameof(Product.Name)} LIKE @SearchTerm");
        }

        return sqlClauseBuilder.ToString();
    }
}
