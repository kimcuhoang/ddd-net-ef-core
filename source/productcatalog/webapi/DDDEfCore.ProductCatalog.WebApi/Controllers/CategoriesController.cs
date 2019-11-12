using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DDDEfCore.ProductCatalog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
            => this._mediator = mediator;

        /// <summary>
        /// Get Specific CatalogCategory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<GetCatalogCategoryDetailResult> Get(GetCatalogCategoryDetailRequest request)
        {
            return await this._mediator.Send(request);
        }

        /// <summary>
        /// Create Category
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            await this._mediator.Send(command);
            return NoContent();
        }
    }
}