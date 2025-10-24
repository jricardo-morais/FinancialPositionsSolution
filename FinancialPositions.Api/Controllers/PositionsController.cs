using FinancialPositions.Application.Services;
using FinancialPositions.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinancialPositions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IFinancialPositionService _service;

        public PositionsController(IFinancialPositionService service) 
        {
            _service = service;
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<FinancialPosition>>> GetLatestPositionsByClient(string clientId)
        {
            var latestPositions = await _service.GetLatestPositionsByClientAsync(clientId);

            if (!latestPositions.Any())
            {
                return NotFound();
            }

            return Ok(latestPositions);
        }

        [HttpGet("client/{clientId}/summary")]
        public async Task<ActionResult<IEnumerable<ProductSummary>>> GetPositionsSummaryByClient(string clientId)
        {
            var summary = await _service.GetPositionsSummaryByClientAsync(clientId);

            if (!summary.Any())
            {
                return NotFound();
            }

            return Ok(summary);
        }

        [HttpGet("top10")]
        public async Task<ActionResult<IEnumerable<FinancialPosition>>> GetTop10Positions()
        {
            var top10Positions = await _service.GetTop10PositionsAsync();

            if (!top10Positions.Any())
            {
                return NotFound();
            }

            return Ok(top10Positions);
        }
    }
}
