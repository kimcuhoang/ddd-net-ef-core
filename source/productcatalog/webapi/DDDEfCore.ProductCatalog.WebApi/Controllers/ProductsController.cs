using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;
using DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UpdateProductCommand command)
        {
            await this._mediator.Send(command);
            return NoContent();
        }
    }
}