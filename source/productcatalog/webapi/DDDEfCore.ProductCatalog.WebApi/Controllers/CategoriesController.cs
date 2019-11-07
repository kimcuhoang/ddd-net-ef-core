using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
            => this._mediator = mediator;

        [HttpGet]
        public async Task<GetCatalogCategoryDetailResult> Get(GetCatalogCategoryDetailRequest request)
        {
            return await this._mediator.Send(request);
        }

        [HttpPost]
        public async Task Create([FromBody] CreateCategoryCommand command)
        {
            await this._mediator.Send(command);
        }
    }
}