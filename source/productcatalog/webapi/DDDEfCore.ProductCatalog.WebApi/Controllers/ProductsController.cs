using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;
using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection;
using DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;

namespace DDDEfCore.ProductCatalog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
            => this._mediator = mediator;

        /// <summary>
        /// Create Product
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            await this._mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Update specific Product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(ProductId productId, [FromBody] string productName)
        {
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductId = productId,
                ProductName = productName
            };
            await this._mediator.Send(command);
            return NoContent();
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
        public async Task<IActionResult> SearchProducts(string searchTerm, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetProductCollectionRequest
            {
                SearchTerm = searchTerm,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = await this._mediator.Send(request);
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
            var result = await this._mediator.Send(request);
            return Ok(result);
        }
    }
}