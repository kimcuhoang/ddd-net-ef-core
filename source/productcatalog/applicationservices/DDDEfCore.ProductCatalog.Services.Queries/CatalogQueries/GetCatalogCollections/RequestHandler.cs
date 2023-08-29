using Dapper;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections
{
    public sealed class RequestHandler : IRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>
    {
        private readonly SqlServerDbConnectionFactory _dbConnection;
        private readonly IValidator<GetCatalogCollectionRequest> _validator;

        public RequestHandler(SqlServerDbConnectionFactory dbConnection, IValidator<GetCatalogCollectionRequest> validator)
        {
            this._dbConnection = dbConnection;
            this._validator = validator;
        }

        #region Implementation of IRequestHandler<in GetCatalogCollectionsRequest,GetCatalogCollectionsResult>

        public async Task<GetCatalogCollectionResult> Handle(GetCatalogCollectionRequest request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, cancellationToken);

            using var connection = await this._dbConnection.GetConnection(cancellationToken);
            var sqlClauses = new List<string>
            {
                this.SqlClauseForQueryingCatalogs(request),
                this.SqlClauseForCountingCatalogs(request)
            };

            var combinedSqlClause = string.Join("; ", sqlClauses);

            var parameters = new
            {
                Offset = (request.PageIndex - 1) * request.PageSize,
                PageSize = request.PageSize,
                SearchTerm = $"%{request.SearchTerm}%"
            };

            var multiQuery = await connection.QueryMultipleAsync(combinedSqlClause, parameters);

            var catalogs = await multiQuery.ReadAsync<GetCatalogCollectionResult.CatalogItem>();
            var totalCatalogs = await multiQuery.ReadFirstAsync<int>();

            var result = new GetCatalogCollectionResult
            {
                CatalogItems = catalogs,
                TotalCatalogs = totalCatalogs
            };

            return result;
        }

        #endregion

        private string SqlClauseForQueryingCatalogs(GetCatalogCollectionRequest request)
        {
            var fieldsDefinition = new Dictionary<string, string>
            {
                {$"{nameof(GetCatalogCollectionResult.CatalogItem.CatalogId)}", $"{nameof(Catalog)}.Id"},
                {$"{nameof(GetCatalogCollectionResult.CatalogItem.DisplayName)}", $"{nameof(Catalog)}.{nameof(Catalog.DisplayName)}"}
            };
            var groupByFields = string.Join(",", fieldsDefinition.Select(x => x.Value));

            fieldsDefinition.Add($"{nameof(GetCatalogCollectionResult.CatalogItem.TotalCategories)}", 
                 $"COUNT({nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)})");
            var selectedFields = string.Join(",", fieldsDefinition.Select(x => $"{x.Key}={x.Value}"));

            var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
                .Append($" FROM {nameof(Catalog)} AS {nameof(Catalog)}")
                .Append($" LEFT JOIN {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
                .Append($" ON {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)} = {nameof(Catalog)}.Id");

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                sqlClauseBuilder = sqlClauseBuilder
                    .Append($" WHERE {nameof(Catalog)}.{nameof(Catalog.DisplayName)} LIKE @SearchTerm");
            }

            sqlClauseBuilder = sqlClauseBuilder
                .Append($" GROUP BY {groupByFields}")
                .Append($" ORDER BY {nameof(Catalog)}.{nameof(Catalog.DisplayName)} ")
                .Append(" OFFSET @Offset ROWS ")
                .Append(" FETCH NEXT @PageSize ROWS ONLY; ");

            return sqlClauseBuilder.ToString();
        }

        private string SqlClauseForCountingCatalogs(GetCatalogCollectionRequest request)
        {
            var sqlClauseBuilder = new StringBuilder("SELECT COUNT(*)")
                .Append($" FROM {nameof(Catalog)} AS {nameof(Catalog)}");

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                sqlClauseBuilder = sqlClauseBuilder
                    .Append($" WHERE {nameof(Catalog)}.{nameof(Catalog.DisplayName)} LIKE @SearchTerm ");
            }

            return sqlClauseBuilder.ToString();
        }
    }
}
