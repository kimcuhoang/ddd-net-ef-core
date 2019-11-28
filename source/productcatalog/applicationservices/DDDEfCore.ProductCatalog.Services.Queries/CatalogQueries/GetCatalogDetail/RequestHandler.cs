using Dapper;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
    public class RequestHandler : IRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult>
    {
        private readonly SqlServerDbConnectionFactory _dbConnectionFactory;
        private readonly IValidator<GetCatalogDetailRequest> _validator;

        public RequestHandler(SqlServerDbConnectionFactory dbConnectionFactory, IValidator<GetCatalogDetailRequest> validator)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));

            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Implementation of IRequestHandler<in GetCatalogDetailRequest,GetCatalogDetailResult>

        public async Task<GetCatalogDetailResult> Handle(GetCatalogDetailRequest request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            using var connection = await this._dbConnectionFactory.GetConnection(cancellationToken);

            var result = new GetCatalogDetailResult();

            var multiSqlClauses = new List<string>
            {
                this.SelectCatalogSqlClause(),
                this.SelectCatalogCategoriesOfCatalog(request.SearchCatalogCategoryRequest),
                this.SqlForCountOfCatalogCategoriesInCatalog()
            };

            var sqlClause = string.Join(";", multiSqlClauses);
            var searchCategoryRequest = request.SearchCatalogCategoryRequest;

            var parameters = new
            {
                Offset = (searchCategoryRequest.PageIndex - 1) * searchCategoryRequest.PageSize,
                PageSize = searchCategoryRequest.PageSize,
                SearchTerm = $"%{searchCategoryRequest.SearchTerm}%",
                CatalogId = request.CatalogId
            };

            var multiQueries = await connection.QueryMultipleAsync(sqlClause, parameters);
            result.CatalogDetail = await multiQueries.ReadFirstOrDefaultAsync<GetCatalogDetailResult.CatalogDetailResult>() ?? new GetCatalogDetailResult.CatalogDetailResult();
                
            if (!result.IsNull)
            {
                result.CatalogCategories = await multiQueries.ReadAsync<GetCatalogDetailResult.CatalogCategorySearchResult>();
                result.TotalOfCatalogCategories = await multiQueries.ReadFirstAsync<int>();
            }

            return result;
        }

        #endregion

        private string SelectCatalogSqlClause()
        {
            var catalogFields = new List<string>
            {
                "Id",
                nameof(Catalog.DisplayName)
            };

            var sqlStringBuilder = new StringBuilder("SELECT ")
                .Append(string.Join(",", catalogFields))
                .Append($" FROM {nameof(Catalog)}")
                .Append($" WHERE Id = @catalogId");

            return sqlStringBuilder.ToString();
        }

        private string SelectCatalogCategoriesOfCatalog(GetCatalogDetailRequest.CatalogCategorySearchRequest request)
        {
            var catalogCategoryFields = new List<string>
            {
                nameof(CatalogCategory.CatalogCategoryId),
                nameof(CatalogCategory.DisplayName),
                nameof(CatalogCategory.CategoryId)
            }.Select(field => $"{nameof(CatalogCategory)}.{field}").ToList();

            var selectedFieldsForCatalog = string.Join(",", catalogCategoryFields);

            var sqlStringBuilder = new StringBuilder($"SELECT {selectedFieldsForCatalog}")
                .Append($", {nameof(GetCatalogDetailResult.CatalogCategorySearchResult.TotalOfProducts)} = COUNT({nameof(CatalogProduct)}.{nameof(CatalogProduct.CatalogProductId)})")
                .Append($" FROM {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
                .Append($" LEFT JOIN {nameof(CatalogProduct)} AS {nameof(CatalogProduct)}")
                .Append($" ON {nameof(CatalogProduct)}.{nameof(CatalogCategory.CatalogCategoryId)} = {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogCategoryId)}")
                .Append($" WHERE {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)} = @catalogId");

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                sqlStringBuilder = sqlStringBuilder
                    .Append($" AND {nameof(CatalogCategory)}.{nameof(CatalogCategory.DisplayName)} LIKE @SearchTerm");
            }

            sqlStringBuilder = sqlStringBuilder
                .Append($" GROUP BY {selectedFieldsForCatalog}")
                .Append($" ORDER BY {nameof(CatalogCategory)}.{nameof(CatalogCategory.DisplayName)}")
                .Append(" OFFSET @Offset ROWS ")
                .Append(" FETCH NEXT @PageSize ROWS ONLY ");

            return sqlStringBuilder.ToString();
        }

        private string SqlForCountOfCatalogCategoriesInCatalog()
        {
            var sqlStringBuilder = new StringBuilder("SELECT COUNT(*)")
                .Append($" FROM {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
                .Append($" WHERE {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)} = @catalogId");

            return sqlStringBuilder.ToString();
        }
    }
}
