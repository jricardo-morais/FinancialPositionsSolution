namespace FinancialPositions.Domain.Entities
{
    public class FinancialPosition
    {
        public required string PositionId { get; set; }
        public required string ProductId { get; set; }
        public required string ClientId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Value { get; set; }
        public decimal? Quantity { get; set; }
    }
}
