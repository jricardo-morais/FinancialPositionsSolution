using FinancialPositions.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialPositions.Application.Services
{
    public interface IFinancialPositionService
    {
        Task AddRangeAsync(IEnumerable<FinancialPosition> positions);
        Task<IEnumerable<FinancialPosition>> GetLatestPositionsByClientAsync(string clientId);
        Task<IEnumerable<ProductSummary>> GetPositionsSummaryByClientAsync(string clientId);
        Task<IEnumerable<FinancialPosition>> GetTop10PositionsAsync();
    }
}