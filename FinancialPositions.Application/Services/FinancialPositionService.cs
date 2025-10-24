using FinancialPositions.Domain.Entities;
using FinancialPositions.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialPositions.Application.Services
{
    public class FinancialPositionService : IFinancialPositionService
    {
        private readonly IFinancialPositionRepository _repository;

        public FinancialPositionService(IFinancialPositionRepository repository)
        {
            _repository = repository;
        }

        public Task AddRangeAsync(IEnumerable<FinancialPosition> positions)
        {
            return _repository.AddRangeAsync(positions);
        }

        public Task<IEnumerable<FinancialPosition>> GetLatestPositionsByClientAsync(string clientId)
        {
            return _repository.GetLatestPositionsByClientAsync(clientId);
        }

        public Task<IEnumerable<ProductSummary>> GetPositionsSummaryByClientAsync(string clientId)
        {
            return _repository.GetPositionsSummaryByClientAsync(clientId);
        }

        public Task<IEnumerable<FinancialPosition>> GetTop10PositionsAsync()
        {
            return _repository.GetTop10PositionsAsync();
        }
    }
}