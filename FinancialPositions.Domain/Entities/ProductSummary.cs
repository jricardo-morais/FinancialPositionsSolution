namespace FinancialPositions.Domain.Entities
{
    public class ProductSummary
    {
        public required string ProductId { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
