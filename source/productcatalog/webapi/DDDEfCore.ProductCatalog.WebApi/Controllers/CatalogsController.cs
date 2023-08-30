﻿using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDDEfCore.ProductCatalog.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogsController(IMediator mediator)
        => this._mediator = mediator;

    /// <summary>
    /// Create new Catalog
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCatalogCommand command)
    {
        await this._mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Update specific Catalog
    /// </summary>
    /// <param name="catalogId"></param>
    /// <param name="catalogName"></param>
    /// <returns></returns>
    [HttpPut("{catalogId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(CatalogId catalogId, [FromBody] string catalogName)
    {
        UpdateCatalogCommand command = new UpdateCatalogCommand
        {
            CatalogId = catalogId,
            CatalogName = catalogName
        };
        await this._mediator.Send(command);
        return NoContent();
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
    public async Task<IActionResult> SearchCatalogsByName(string searchTerm = null, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetCatalogCollectionRequest
        {
            SearchTerm = searchTerm,
            PageSize = pageSize,
            PageIndex = pageIndex
        };
        var result = await this._mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Search Catalog by CatalogId and also its CatalogCategory
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetCatalogDetailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchSpecificCatalogByCatalogId(GetCatalogDetailRequest request)
    {
        var result = await this._mediator.Send(request);
        return Ok(result);
    }
}