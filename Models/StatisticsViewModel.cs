namespace InventarioInteligente.Models
{
    public class StatisticsViewModel
    {
        public List<Product> OrderedByPriceDesc { get; set; } = new();
        public decimal AveragePrice { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<Product> LowStockProducts { get; set; } = new();
    }
}
