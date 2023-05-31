using System.ComponentModel.DataAnnotations;

namespace BrokerApp
{
    public class OrderDB
    {
        [Key]
        public int OrderID { get; set; }
        public string OrderType { get; set; }
        public int StockID { get; set; }
        public int UserID { get; set; }
        public int Quantity { get; set; }
    }
}