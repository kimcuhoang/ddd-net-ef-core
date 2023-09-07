using System.Data;
using System.Text;
using Dapper;
using DDD.ProductCatalog.Core.Catalogs;
using MediatR;

namespace DDD.ProductCatalog.Application.Queries.CatalogProductQueries.GetCatalogProductDetail;

public class RequestHandler : IRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>
{
    private readonly IDbConnection _dbConnection;

    public RequestHandler(IDbConnection dbConnection)
    {
        this._dbConnection = dbConnection;
    }

    #region Implementation of IRequestHandler<in GetCatalogProductDetailRequest,GetCatalogProductDetailResult>

    public async Task<GetCatalogProductDetailResult> Handle(GetCatalogProductDetailRequest request, CancellationToken cancellationToken)
    {
        var result = new GetCatalogProductDetailResult
        {
            CatalogCategory = new GetCatalogProductDetailResult.CatalogCategoryInfo(),
            Catalog = new GetCatalogProductDetailResult.CatalogInfo()
        };

        var sqlClauses = new List<string>
            {
                this.SqlClauseForQueryingCatalogProduct(),
                this.SqlClauseForQueryingCatalogCategory(),
                this.SqlClauseForQueryingCatalog()
            };

        var parameters = new
        {
            request.CatalogProductId
        };

        var multiQueries = await this._dbConnection.QueryMultipleAsync(string.Join("; ", sqlClauses), parameters);

        result.CatalogProduct =
            await multiQueries.ReadFirstOrDefaultAsync<GetCatalogProductDetailResult.CatalogProductInfo>() ??
            new GetCatalogProductDetailResult.CatalogProductInfo();

        if (!result.IsNull)
        {
            result.CatalogCategory = await multiQueries
                .ReadFirstOrDefaultAsync<GetCatalogProductDetailResult.CatalogCategoryInfo>();
            result.Catalog = await multiQueries
                .ReadFirstOrDefaultAsync<GetCatalogProductDetailResult.CatalogInfo>();
        }

        return result;
    }

    #endregion

    private string SqlClauseForQueryingCatalogProduct()
    {
        var fieldsDefinition = new Dictionary<string, string>
        {
            {
                $"{nameof(GetCatalogProductDetailResult.CatalogProduct.CatalogProductId)}",
                $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Id)}"
            },
            {
                $"{nameof(GetCatalogProductDetailResult.CatalogProduct.DisplayName)}",
                $"{nameof(CatalogProduct)}.{nameof(CatalogProduct.DisplayName)}"
            }
        };

        var selectedFields = string.Join(", ", fieldsDefinition.Select(x => $"{x.Key} = {x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(CatalogProduct)} AS {nameof(CatalogProduct)}")
            .Append($" WHERE {nameof(CatalogProduct)}.{nameof(CatalogProduct.Id)} = @CatalogProductId");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForFindingCatalogCategoryIdOfCatalogProduct =>
        new StringBuilder($"SELECT {nameof(CatalogProduct)}.{nameof(CatalogCategoryId)}")
            .Append($" FROM {nameof(CatalogProduct)} AS {nameof(CatalogProduct)}")
            .Append($" WHERE {nameof(CatalogProduct)}.{nameof(CatalogProduct.Id)} = @CatalogProductId")
            .ToString();


    private string SqlClauseForQueryingCatalogCategory()
    {
        var catalogCategoryFieldsDefinition = new Dictionary<string, string>
        {
            {
                $"{nameof(GetCatalogProductDetailResult.CatalogCategory.CatalogCategoryId)}",
                $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)}"
            },
            {
                $"{nameof(GetCatalogProductDetailResult.CatalogCategory.DisplayName)}",
                $"{nameof(CatalogCategory)}.{nameof(CatalogCategory.DisplayName)}"
            },
        };
        var selectedFields = string.Join(", ", catalogCategoryFieldsDefinition.Select(x => $"{x.Key} = {x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
            .Append(
                $" WHERE {nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)} = ({this.SqlClauseForFindingCatalogCategoryIdOfCatalogProduct})");

        return sqlClauseBuilder.ToString();
    }

    private string SqlClauseForFindingCatalogIdOfCatalogCategory =>
        new StringBuilder($"SELECT {nameof(CatalogCategory)}.{nameof(CatalogCategory.CatalogId)}")
            .Append($" FROM {nameof(CatalogCategory)} AS {nameof(CatalogCategory)}")
            .Append(
                $" WHERE {nameof(CatalogCategory)}.{nameof(CatalogCategory.Id)} = ({this.SqlClauseForFindingCatalogCategoryIdOfCatalogProduct})")
            .ToString();

    private string SqlClauseForQueryingCatalog()
    {
        var fieldsDefinition = new Dictionary<string, string>
        {
            {
                $"{nameof(GetCatalogProductDetailResult.Catalog.CatalogId)}",
                $"{nameof(Catalog)}.Id"
            },
            {
                $"{nameof(GetCatalogProductDetailResult.Catalog.CatalogName)}",
                $"{nameof(Catalog)}.{nameof(Catalog.DisplayName)}"
            }
        };

        var selectedFields = string.Join(", ", fieldsDefinition.Select(x => $"{x.Key} = {x.Value}"));

        var sqlClauseBuilder = new StringBuilder($"SELECT {selectedFields}")
            .Append($" FROM {nameof(Catalog)} AS {nameof(Catalog)}")
            .Append($" WHERE {nameof(Catalog)}.Id = ({SqlClauseForFindingCatalogIdOfCatalogCategory})");

        return sqlClauseBuilder.ToString();
    }
}
