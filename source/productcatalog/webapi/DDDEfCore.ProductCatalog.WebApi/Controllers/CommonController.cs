using DDDEfCore.ProductCatalog.Services.Commands.MigrateDatabaseCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CommonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommonController(IMediator mediator)
            => this._mediator = mediator;

        public async Task<IActionResult> Migrate()
        {
            try
            {
                await this._mediator.Send(new MigrateDatabaseCommand());
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}