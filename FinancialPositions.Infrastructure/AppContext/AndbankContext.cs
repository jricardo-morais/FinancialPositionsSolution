using FinancialPositions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialPositions.Infrastructure.AppContext
{
    public class AndbankContext : DbContext
    {
        public DbSet<FinancialPosition> Positions { get; set; }

        public AndbankContext(DbContextOptions<AndbankContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FinancialPosition>()
                .HasKey(p => new { p.PositionId, p.Date });
        }
    }
}
