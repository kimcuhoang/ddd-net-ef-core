using MediatR;
using Microsoft.AspNetCore.Mvc;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryDetail;
using DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryCollection;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.Application.Commands.CategoryCommands.UpdateCategory;
using DDD.ProductCatalog.Application.Commands.CategoryCommands.CreateCategory;

namespace DDD.ProductCatalog.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
        => this._sender = sender;

    /// <summary>
    /// Create CategoryDetail
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await this._sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update Specific CategoryDetail
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    [HttpPut("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(CategoryId categoryId, [FromBody] string categoryName)
    {
        UpdateCategoryCommand command = new UpdateCategoryCommand
        {
            CategoryId = categoryId,
            CategoryName = categoryName
        };
        var result = await this._sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Search Categories by Name
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(GetCategoryCollectionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchCatalogsByName(string searchTerm = null, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetCategoryCollectionRequest
        {
            SearchTerm = searchTerm,
            PageSize = pageSize,
            PageIndex = pageIndex
        };
        var result = await this._sender.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Get specific CategoryDetail
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpGet("{categoryId}")]
    [ProducesResponseType(typeof(GetCategoryDetailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpecificCategory(CategoryId categoryId)
    {
        var request = new GetCategoryDetailRequest { CategoryId = categoryId };
        var result = await this._sender.Send(request);
        return Ok(result);
    }
}