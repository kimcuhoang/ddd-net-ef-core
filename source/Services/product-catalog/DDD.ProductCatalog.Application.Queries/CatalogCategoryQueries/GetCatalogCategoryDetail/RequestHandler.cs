﻿using System.Data;
using System.Text;
using Dapper;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;
using MediatR;

namespace DDD.ProductCatalog.Application.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;

public class RequestHandler : IRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>
{
    public readonly IDbConnection _dbConnection;

    public RequestHandler(IDbConnection dbConnection)
    {
        this._dbConnection = dbConnection;
    }

    #region Implementation of IRequestHandler<in GetCatalogCategoryDetailRequest,GetCatalogCategoryDetailResult>

    public async Task<GetCatalogCategoryDetailResult> Handle(GetCatalogCategoryDetailRequest request, CancellationToken cancellationToken)
    {
        var result = new GetCatalogCategoryDetailResult();

        var multiSqlClauses = new List<string>
            {
                this.SqlClauseForQueryingCatalogCategory(),
                this.SqlClauseForQueryingCatalogProducts(request.CatalogProductCriteria),
                this.SqlClauseForCountingCatalogProducts(request.CatalogProductCriteria)
            };

        var sqlClauses = string.Join(";", multiSqlClauses);

        var parameters = new
        {
            Offset = Math.Abs((request.CatalogProductCriteria.PageIndex - 1) *
                              request.CatalogProductCriteria.PageSize),
            PageSize = request.CatalogProductCriteria.PageSize == 0
                ? request.CatalogProductCriteria.PageSize + 1
                : request.CatalogProductCriteria.PageSize,
            SearchTerm = $"%{request.CatalogProductCriteria.SearchTerm}%",
            request.CatalogCategoryId
        };

        var multiQueries = await this._dbConnection.QueryMultipleAsync(sqlClauses, parameters);

        result.CatalogCategoryDetail = await multiQueries
                .ReadFirstOrDefaultAsync<GetCatalogCategoryDetailResult.CatalogCategoryDetailResult>() ??
                new GetCatalogCategoryDetailResult.CatalogCategoryDetailResult();

        if (!result.CatalogCategoryDetail.IsNull)
        {
            result.CatalogProducts = await multiQueries.ReadAsync<GetCatalogCategoryDetailResult.CatalogProductResult>();
            result.TotalOfCatalogProducts = await multiQueries.ReadFirstAsync<int>();
        }

        return result;
    }

    #endregion

    private string SqlClauseForQueryingCatalogCategory()
    {
        var selectedFields = new List<string>
        {
            $"{nameof(GetCatalogCategoryDetailResult.CatalogCategoryDetail.CatalogId)} = {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}",
            $"{nameof(GetCatalogCategoryDetailResult.CatalogCategoryDetail.CatalogCategoryId)} = {nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)}",
            $"{nameof(GetCatalogCategoryDetailResult.CatalogCategoryDetail.CatalogCategoryName)} = {nameof(CatalogCategory)}.{nameof(CatalogCategory.DisplayName)}",
            $"{nameof(GetCatalogCategoryDetailResult.CatalogCategoryDetail.CatalogName)} = {nameof(Catalog)}.{nameof(Catalog.DisplayName)}"
        };

        var sqlSelectedFields = string.Join(",", selectedFields);

        var sqlClauseBuilder = new StringBuilder($"SELECT {sqlSelectedFields}")
            .Append($" FROM {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
            .Append($" INNER JOIN {nameof(Catalog)} AS {nameof(Catalog)}")
            .Append($" ON {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)} = {nameof(Catalog)}.Id")
            .Append($" WHERE {nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)} = @CatalogCategoryId");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForQueryingCatalogProducts(GetCatalogCategoryDetailRequest.CatalogProductSearchRequest searchRequest)
    {
        var fieldsDefinition = new Dictionary<string, string>
        {
            {$"{nameof(GetCatalogCategoryDetailResult.CatalogProductResult.CatalogProductId)}", $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Id)}"},
            {$"{nameof(GetCatalogCategoryDetailResult.CatalogProductResult.DisplayName)}",  $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)}"},
            {$"{nameof(GetCatalogCategoryDetailResult.CatalogProductResult.ProductId)}", $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.ProductId)}"},
            {$"{nameof(GetCatalogCategoryDetailResult.CatalogProductResult.ProductName)}", $"{nameof(Product)}.{nameof(Product.Name)}"},
        };

        var sqlSelectedFields = string.Join(",", fieldsDefinition.Select(x => $"{x.Key}={x.Value}"));
        var groupByFields = string.Join(",", fieldsDefinition.Select(x => x.Value));

        var sqlClauseBuilder = new StringBuilder($"SELECT {sqlSelectedFields}")
            .Append($" FROM {nameof(CatalogProduct)} AS {nameof(CatalogProduct)}")
            .Append($" INNER JOIN {nameof(Product)} AS {nameof(Product)}")
            .Append($" ON {nameof(CatalogProduct)}.{nameof(CatalogProduct.ProductId)} = {nameof(Product)}.Id")
            .Append($" WHERE {nameof(CatalogProduct)}.{nameof(CatalogCategoryId)} = @CatalogCategoryId");

        if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
        {
            sqlClauseBuilder = sqlClauseBuilder
                .Append($" AND {nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)} LIKE @SearchTerm");
        }

        sqlClauseBuilder = sqlClauseBuilder
            .Append($" GROUP BY {groupByFields}")
            .Append($" ORDER BY {nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)}")
            .Append(" OFFSET @Offset ROWS ")
            .Append(" FETCH NEXT @PageSize ROWS ONLY ");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForCountingCatalogProducts(GetCatalogCategoryDetailRequest.CatalogProductSearchRequest searchRequest)
    {
        var sqlClauseBuilder = new StringBuilder($"SELECT COUNT(*)")
            .Append($" FROM {nameof(CatalogProduct)}")
            .Append($" WHERE {nameof(CatalogCategoryId)} = @CatalogCategoryId");

        if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
        {
            sqlClauseBuilder = sqlClauseBuilder
                .Append($" AND {nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)} LIKE @SearchTerm");
        }

        return sqlClauseBuilder.ToString();
    }
}
