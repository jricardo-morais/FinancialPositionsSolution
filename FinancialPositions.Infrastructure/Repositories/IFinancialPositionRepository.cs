using FinancialPositions.Domain.Entities;

namespace FinancialPositions.Infrastructure.Repositories
{
    public interface IFinancialPositionRepository
    {
        Task AddRangeAsync(IEnumerable<FinancialPosition> positions);
        Task<IEnumerable<FinancialPosition>> GetLatestPositionsByClientAsync(string clientId);
        Task<IEnumerable<ProductSummary>> GetPositionsSummaryByClientAsync(string clientId);
        Task<IEnumerable<FinancialPosition>> GetTop10PositionsAsync();
    }
}
