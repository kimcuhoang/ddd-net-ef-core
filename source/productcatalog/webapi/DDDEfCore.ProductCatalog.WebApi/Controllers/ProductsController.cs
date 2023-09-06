using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;
using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDDEfCore.ProductCatalog.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
        => this._sender = sender;

    /// <summary>
    /// Create Product
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await this._sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update specific Product
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="productName"></param>
    /// <returns></returns>
    [HttpPut("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(ProductId productId, [FromBody] string productName)
    {
        UpdateProductCommand command = new UpdateProductCommand
        {
            ProductId = productId,
            ProductName = productName
        };
        var result = await this._sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Search Products by Name
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(GetProductCollectionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchProducts(string? searchTerm = null, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetProductCollectionRequest
        {
            SearchTerm = searchTerm,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await this._sender.Send(request);
        return Ok(result);
    }
    
    /// <summary>
    /// Get detail of specific Product
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(GetProductDetailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GlobalExceptionHandlerMiddleware.ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductDetail(ProductId productId)
    {
        var request = new GetProductDetailRequest { ProductId = productId};
        var result = await this._sender.Send(request);
        return Ok(result);
    }
}