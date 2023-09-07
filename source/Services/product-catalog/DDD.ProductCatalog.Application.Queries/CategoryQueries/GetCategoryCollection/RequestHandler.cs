using System.Text;
using MediatR;
using Dapper;
using DDD.ProductCatalog.Core.Categories;
using System.Data;

namespace DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryCollection;

public class RequestHandler : IRequestHandler<GetCategoryCollectionRequest, GetCategoryCollectionResult>
{
    private readonly IDbConnection _dbConnection;

    public RequestHandler(IDbConnection dbConnection)
    {
        this._dbConnection = dbConnection;
    }

    #region Implementation of IRequestHandler<in GetCategoryCollectionRequest,GetCategoryCollectionResult>

    public async Task<GetCategoryCollectionResult> Handle(GetCategoryCollectionRequest request, CancellationToken cancellationToken)
    {

        var parameters = new
        {
            Offset = (request.PageIndex - 1) * request.PageSize,
            request.PageSize,
            SearchTerm = $"%{request.SearchTerm}%"
        };

        var sqlClauses = new List<string>
        {
            this.SqlClauseForQueryingCategories(request),
            this.SqlClauseForCountingCategories(request)
        };
        var combinedSqlClause = string.Join("; ", sqlClauses);

        var multiQuery = await this._dbConnection.QueryMultipleAsync(combinedSqlClause, parameters);

        var categories = await multiQuery.ReadAsync<GetCategoryCollectionResult.CategoryResult>();
        var totalCategories = await multiQuery.ReadFirstAsync<int>();

        var result = new GetCategoryCollectionResult
        {
            Categories = categories,
            TotalCategories = totalCategories
        };

        return result;
    }

    #endregion

    private string SqlClauseForQueryingCategories(GetCategoryCollectionRequest request)
    {
        var fieldsDefinition = new Dictionary<string, string>
        {
            {$"{nameof(GetCategoryCollectionResult.CategoryResult.Id)}", $"{nameof(Category)}.Id"},
            {$"{nameof(GetCategoryCollectionResult.CategoryResult.DisplayName)}", $"{nameof(Category)}.{nameof(Category.DisplayName)}"}
        };
        var groupByFields = string.Join(",", fieldsDefinition.Select(x => x.Value));
        var selectedFields = string.Join(",", fieldsDefinition.Select(x => $"{x.Key}={x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(Category)} AS {nameof(Category)}");

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            sqlClauseBuilder = sqlClauseBuilder
                .Append($" WHERE {nameof(Category)}.{nameof(Category.DisplayName)} LIKE @SearchTerm");
        }

        sqlClauseBuilder = sqlClauseBuilder
            .Append($" GROUP BY {groupByFields}")
            .Append($" ORDER BY {nameof(Category)}.{nameof(Category.DisplayName)} ")
            .Append(" OFFSET @Offset ROWS ")
            .Append(" FETCH NEXT @PageSize ROWS ONLY; ");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForCountingCategories(GetCategoryCollectionRequest request)
    {
        var sqlClauseBuilder = new StringBuilder("SELECT COUNT(*)")
            .Append($" FROM {nameof(Category)} AS {nameof(Category)}");

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            sqlClauseBuilder = sqlClauseBuilder
                .Append($" WHERE {nameof(Category)}.{nameof(Category.DisplayName)} LIKE @SearchTerm ");
        }

        return sqlClauseBuilder.ToString();
    }
}
