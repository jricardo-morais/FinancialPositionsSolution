using FinancialPositions.Domain.Entities;
using FinancialPositions.Infrastructure.AppContext;
using Microsoft.EntityFrameworkCore;

namespace FinancialPositions.Infrastructure.Repositories
{
    public class FinancialPositionRepository : IFinancialPositionRepository
    {
        private readonly AndbankContext _context;

        public FinancialPositionRepository(AndbankContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<FinancialPosition> positions)
        {
            await _context.Positions.AddRangeAsync(positions);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FinancialPosition>> GetLatestPositionsByClientAsync(string clientId)
        {
            return await GetLatestPositionsQueryable(clientId).ToListAsync();
        }

        public async Task<IEnumerable<ProductSummary>> GetPositionsSummaryByClientAsync(string clientId)
        {
            return await GetLatestPositionsQueryable(clientId)
                .GroupBy(p => p.ProductId)
                .Select(g => new ProductSummary
                {
                    ProductId = g.Key,
                    TotalValue = (g.Sum(p => p.Value) ?? 0m),
                    TotalQuantity = (g.Sum(p => p.Quantity) ?? 0m)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialPosition>> GetTop10PositionsAsync()
        {
            return await _context.Positions.AsQueryable()
                .OrderByDescending(p => p.Value)
                .Take(10)
                .ToListAsync();
        }

        // Helper para reutilizar a buscar das últimas posicoes por clienteId.
        private IQueryable<FinancialPosition> GetLatestPositionsQueryable(string? clientId = null)
        {
            var positions = _context.Positions.Where(p => p.ClientId == clientId).AsQueryable();

            if (!positions.Any())
            {
                return Enumerable.Empty<FinancialPosition>().AsQueryable();
            }

            //Obtem as ultimas posicoes agrupadas pelo positionId e ordenando por data  
            var lastPositions = positions
                .GroupBy(p => p.PositionId)
                .Select(lp => new { PositionId = lp.Key, MaxDate = lp.Max(x => x.Date) });

            // Join com lastPositions para obter as posicoes completas
            var query = from p in positions
                        join lp in lastPositions on new { p.PositionId, p.Date } equals new { lp.PositionId, Date = lp.MaxDate }
                        select p;

            return query;
        }
    }
}
