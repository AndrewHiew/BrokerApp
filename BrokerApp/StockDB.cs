using System.ComponentModel.DataAnnotations;

namespace BrokerApp
{
    public class StockDB
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string TickerSymbol { get; set; }
        public double CurrentValue { get; set; }
    }
}