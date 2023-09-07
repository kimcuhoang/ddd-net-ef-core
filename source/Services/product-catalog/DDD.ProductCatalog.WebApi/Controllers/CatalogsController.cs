using MediatR;
using Microsoft.AspNetCore.Mvc;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogDetail;
using DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogCollections;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.Application.Commands.CatalogCommands.UpdateCatalog;
using DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalog;

namespace DDD.ProductCatalog.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogsController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogsController(ISender sender)
        => this._sender = sender;

    /// <summary>
    /// Create new Catalog
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCatalogCommand command)
    {
        var response = await this._sender.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Update specific Catalog
    /// </summary>
    /// <param name="catalogId"></param>
    /// <param name="catalogName"></param>
    /// <returns></returns>
    [HttpPut("{catalogId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(CatalogId catalogId, [FromBody] string catalogName)
    {
        UpdateCatalogCommand command = new UpdateCatalogCommand
        {
            CatalogId = catalogId,
            CatalogName = catalogName
        };
        var result = await this._sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Search Catalogs by Name
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(GetCatalogCollectionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchCatalogsByName(string? searchTerm = null, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetCatalogCollectionRequest
        {
            SearchTerm = searchTerm,
            PageSize = pageSize,
            PageIndex = pageIndex
        };
        var result = await this._sender.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Search Catalog by CatalogId and also its CatalogCategory
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetCatalogDetailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchSpecificCatalogByCatalogId(GetCatalogDetailRequest request)
    {
        var result = await this._sender.Send(request);
        return Ok(result);
    }
}