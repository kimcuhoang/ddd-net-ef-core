using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections
{
    public sealed class RequestHandler : IRequestHandler<GetCatalogCollectionRequest, GetCatalogCollectionResult>
    {
        private readonly SqlServerDbConnectionFactory _dbConnection;

        public RequestHandler(SqlServerDbConnectionFactory dbConnection)
        {
            this._dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        #region Implementation of IRequestHandler<in GetCatalogCollectionsRequest,GetCatalogCollectionsResult>

        public async Task<GetCatalogCollectionResult> Handle(GetCatalogCollectionRequest request, CancellationToken cancellationToken)
        {
            using (var connection = await this._dbConnection.GetConnection(cancellationToken))
            {
                var sqlClauseBuilder = 
                    new StringBuilder($@"SELECT {nameof(GetCatalogCollectionResult.CatalogItem.CatalogId)} = {nameof(Catalog)}.Id, {nameof(Catalog)}.{nameof(Catalog.DisplayName)}, {nameof(GetCatalogCollectionResult.CatalogItem.TotalCategories)} = COUNT({nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogCategoryId)}) ")
                        .Append($"FROM {nameof(Catalog)} AS {nameof(Catalog)} ")
                        .Append($"LEFT JOIN {nameof(CatalogCategory)} AS {nameof(CatalogCategory)} ON {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)} = {nameof(Catalog)}.Id ");
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    sqlClauseBuilder = sqlClauseBuilder
                        .Append($"WHERE {nameof(Catalog)}.{nameof(Catalog.DisplayName)} LIKE @SearchTerm ");
                }

                sqlClauseBuilder = sqlClauseBuilder
                    .Append($"GROUP BY {nameof(Catalog)}.Id, {nameof(Catalog)}.{nameof(Catalog.DisplayName)} ")
                    .Append($"ORDER BY {nameof(Catalog)}.{nameof(Catalog.DisplayName)} ")
                    .Append("OFFSET @Offset ROWS ")
                    .Append("FETCH NEXT @PageSize ROWS ONLY; ");

                sqlClauseBuilder = sqlClauseBuilder
                        .Append($"SELECT COUNT(*) FROM {nameof(Catalog)} AS {nameof(Catalog)} ");

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    sqlClauseBuilder = sqlClauseBuilder
                        .Append($"WHERE {nameof(Catalog)}.{nameof(Catalog.DisplayName)} LIKE @SearchTerm ");
                }

                var parameters = new
                {
                    Offset = Math.Abs((request.PageIndex - 1) * request.PageSize),
                    PageSize = request.PageSize == 0 ? request.PageSize + 1 : request.PageSize,
                    SearchTerm = $"%{request.SearchTerm}%"
                };

                var multiQuery = await connection.QueryMultipleAsync(sqlClauseBuilder.ToString(), parameters);

                var catalogs = await multiQuery.ReadAsync<GetCatalogCollectionResult.CatalogItem>();
                var totalCatalogs = await multiQuery.ReadFirstAsync<int>();

                var result = new GetCatalogCollectionResult
                {
                    CatalogItems = catalogs,
                    TotalCatalogs = totalCatalogs
                };

                return result;
            }
        }

        #endregion
    }
}
