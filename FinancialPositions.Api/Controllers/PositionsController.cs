using FinancialPositions.Domain.Entities;
using FinancialPositions.Infrastructure.Repositories; 
using Microsoft.AspNetCore.Mvc;

namespace FinancialPositions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IFinancialPositionRepository _repository;

        public PositionsController(IFinancialPositionRepository repository) 
        {
            _repository = repository;
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<FinancialPosition>>> GetLatestPositionsByClient(string clientId)
        {
            var latestPositions = await _repository.GetLatestPositionsByClientAsync(clientId);

            if (!latestPositions.Any())
            {
                return NotFound();
            }

            return Ok(latestPositions);
        }

        [HttpGet("client/{clientId}/summary")]
        public async Task<ActionResult<IEnumerable<ProductSummary>>> GetPositionsSummaryByClient(string clientId)
        {
            var summary = await _repository.GetPositionsSummaryByClientAsync(clientId);

            if (!summary.Any())
            {
                return NotFound();
            }

            return Ok(summary);
        }

        [HttpGet("top10")]
        public async Task<ActionResult<IEnumerable<FinancialPosition>>> GetTop10Positions()
        {
            var top10Positions = await _repository.GetTop10PositionsAsync();

            if (!top10Positions.Any())
            {
                return NotFound();
            }

            return Ok(top10Positions);
        }
    }
}
