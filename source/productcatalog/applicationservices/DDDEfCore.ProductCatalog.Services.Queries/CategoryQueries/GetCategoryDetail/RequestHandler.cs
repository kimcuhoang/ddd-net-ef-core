using Dapper;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail
{
    public class RequestHandler : IRequestHandler<GetCategoryDetailRequest, GetCategoryDetailResult>
    {
        private readonly SqlServerDbConnectionFactory _connectionFactory;
        private readonly IValidator<GetCategoryDetailRequest> _validator;

        public RequestHandler(SqlServerDbConnectionFactory connectionFactory,
            IValidator<GetCategoryDetailRequest> validator)
        {
            this._connectionFactory = connectionFactory;
            this._validator = validator;
        }

        #region Implementation of IRequestHandler<in GetCategoryDetailRequest,GetCategoryDetailResult>

        public async Task<GetCategoryDetailResult> Handle(GetCategoryDetailRequest request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var sqlClauses = new List<string>
            {
                this.SqlClauseForQueryingCategory(),
                this.SqlClauseForQueryingCatalogsOfCategory()
            };

            var combinedSqlClause = string.Join(";", sqlClauses);

            var parameters = new
            {
                CategoryId = request.CategoryId
            };

            using var connection = await this._connectionFactory.GetConnection(cancellationToken);

            var multiQueries = await connection.QueryMultipleAsync(combinedSqlClause, parameters);
            var category = await multiQueries.ReadFirstOrDefaultAsync<GetCategoryDetailResult.CategoryDetailResult>();
            var catalogs = await multiQueries.ReadAsync<GetCategoryDetailResult.CatalogOfCategoryResult>();

            var result = new GetCategoryDetailResult
            {
                CategoryDetail = category ?? new GetCategoryDetailResult.CategoryDetailResult(),
                AssignedToCatalogs = catalogs ?? new List<GetCategoryDetailResult.CatalogOfCategoryResult>()
            };

            return result;
        }

        #endregion

        private string SqlClauseForQueryingCategory()
        {
            var fieldsDefinition = new Dictionary<string, string>
            {
                { nameof(GetCategoryDetailResult.CategoryDetail.Id), $"{nameof(Category)}.Id"},
                { nameof(GetCategoryDetailResult.CategoryDetail.DisplayName), $"{nameof(Category)}.{nameof(Category.DisplayName)}" }
            };

            var selectedFields = string.Join(",", fieldsDefinition.Select(x => $"{x.Key}={x.Value}"));

            var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
                .Append($" FROM {nameof(Category)} AS {nameof(Category)}")
                .Append($" WHERE {nameof(Category)}.Id = @CategoryId");

            return sqlClauseBuilder.ToString();
        }

        private string SqlClauseForQueryingCatalogsOfCategory()
        {
            var fieldsDefinition = new Dictionary<string, string>
            {
                { nameof(GetCategoryDetailResult.CatalogOfCategoryResult.Id), $"{nameof(Catalog)}.Id"},
                { nameof(GetCategoryDetailResult.CatalogOfCategoryResult.DisplayName), $"{nameof(Catalog)}.{nameof(Catalog.DisplayName)}" }
            };

            var selectedFields = string.Join(",", fieldsDefinition.Select(x => $"{x.Key}={x.Value}"));

            var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
                .Append($" FROM {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
                .Append($" INNER JOIN {nameof(Catalog)} AS {nameof(Catalog)}")
                .Append($" ON {nameof(Catalog)}.Id = {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}")
                .Append($" WHERE {nameof(CatalogCategory)}.{nameof(CatalogCategory.CategoryId)} = @CategoryId");

            return sqlClauseBuilder.ToString();
        }
    }
}
