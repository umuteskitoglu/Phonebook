using Application.Features.Reports.Commands;
using Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ReportServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all reports.
        /// </summary>
        /// <returns>List of all reports</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var query = new GetReportsQuery();
            var result = await _mediator.Send(query);

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            if (result.Data == null || result.Data.Count == 0)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Gets a report by ID with its details.
        /// </summary>
        /// <param name="query">GetReportByIdQuery</param>
        /// <returns>The report with its details</returns>
        [HttpPost]
        public async Task<IActionResult> GetReportById(GetReportByIdQuery query)
        {
            var result = await _mediator.Send(query);

            if (!result.Success)
                return result.Message.Contains("not found") ? NotFound(result) : StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(result.Data);
        }
    }
}