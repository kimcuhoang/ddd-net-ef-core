using Dapper;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.Db;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail
{
    public class RequestHandler : IRequestHandler<GetProductDetailRequest, GetProductDetailResult>
    {
        private readonly SqlServerDbConnectionFactory _connectionFactory;
        private readonly IValidator<GetProductDetailRequest> _validator;

        public RequestHandler(SqlServerDbConnectionFactory connectionFactory,
            IValidator<GetProductDetailRequest> validator)
        {
            this._connectionFactory = connectionFactory;
            this._validator = validator;
        }


        #region Implementation of IRequestHandler<in GetProductDetailRequest,GetProductDetailResult>

        public async Task<GetProductDetailResult> Handle(GetProductDetailRequest request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var sqlClauses = new List<string>
            {
                this.SqlClauseForQueryingProduct(),
                this.SqlClauseForQueryingCatalogCategory()
            };
            var combinedSqlClauses = string.Join("; ", sqlClauses);
            var parameters = new {ProductId = request.ProductId};

            using var connection = await this._connectionFactory.GetConnection(cancellationToken);
            var multiQueries = await connection.QueryMultipleAsync(combinedSqlClauses, parameters);

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
            var fields = new Dictionary<string,string>
            {
                { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogCategoryId), $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogCategoryId)}" },
                { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogCategoryName), $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.DisplayName)}" },
                { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogId), $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}" },
                { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogName), $"{nameof(Catalog)}.{nameof(Catalog.DisplayName)}" },
                { nameof(GetProductDetailResult.CatalogCategoryResult.ProductDisplayName), $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)}" },
                { nameof(GetProductDetailResult.CatalogCategoryResult.CatalogProductId), $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.CatalogProductId)}" }
            };

            var selectedFields = string.Join(", ", fields.Select(x => $"{x.Key}={x.Value}"));

            var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
                .Append($" FROM {nameof(CatalogProduct)} AS {nameof(CatalogProduct)}")
                .Append($" INNER JOIN {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
                .Append($" ON {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogCategoryId)} = {nameof(CatalogProduct)}.{nameof(CatalogCategory)}Id")
                .Append($" INNER JOIN {nameof(Catalog)} AS {nameof(Catalog)}")
                .Append($" ON {nameof(Catalog)}.Id = {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}")
                .Append($" WHERE {nameof(CatalogProduct)}.{nameof(CatalogProduct.ProductId)} = @ProductId");

            return sqlClauseBuilder.ToString();
        }
    }
}
