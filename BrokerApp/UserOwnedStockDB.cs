using System.ComponentModel.DataAnnotations;

namespace BrokerApp
{
    public class UserOwnedStockDB
    {
        [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public string OwnedType { get; set; }
        public int StockID { get; set; }
        public int Quantity { get; set; }
        public double TotalValue { get; set; }
        public double StockBoughtValue { get; set; }
    }
}