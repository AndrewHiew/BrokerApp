using System.ComponentModel.DataAnnotations;

namespace BrokerApp
{
    
    public class TradeDB
    {
        [Key]
        public int TradeID { get; set; }
        public string TradeType { get; set; }
        public int StockID { get; set; }
        public int UserID { get; set; }
        public int Quantity { get; set; }
        public double BuyPrice { get; set; }
        public double TotalBuyValue { get; set; }
        public double UserCurrentBalance { get; set; }
        public double UserTotalBalance { get; set; }
    }
}