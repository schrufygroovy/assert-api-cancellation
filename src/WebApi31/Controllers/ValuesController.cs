using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace WebApi31.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("haxi")]
        public async Task<IActionResult> GetHaxi(
            CancellationToken cancellationToken,
            [FromServices]IHaxiRepository haxiRepository,
            [FromQuery]Guid[] haxiIds)
        {
            var dataSet = await haxiRepository.Get(
                haxiIds,
                cancellationToken);
            return this.Ok(dataSet);
        }
    }
}
