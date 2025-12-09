using Microsoft.AspNetCore.Mvc;
using NorthWind.DomainLogs.Entities.Dtos;
using NorthWind.DomainLogs.Entities.Interfaces;

namespace NorthWind.Sales.Backend.Controllers.Logs
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController(IDomainLogsRepository repository) : ControllerBase
    {
        [HttpGet("domain")]
        public async Task<ActionResult<PaginatedLogsDto<DomainLogDto>>> GetDomainLogs(
            [FromQuery] int page = 1,
            [FromQuery] int take = 10)
        {
            var result = await repository.GetDomainLogsPaged(page, take);
            return Ok(result);
        }

        [HttpGet("errors")]
        public async Task<ActionResult<PaginatedLogsDto<ErrorLogDto>>> GetErrorLogs(
            [FromQuery] int page = 1,
            [FromQuery] int take = 10)
        {
            var result = await repository.GetErrorLogsPaged(page, take);
            return Ok(result);
        }
    }
}
