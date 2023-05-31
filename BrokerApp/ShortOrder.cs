using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerApp
{
    public class ShortOrder : Order
    {
        public ShortOrder(int orderID, Stock stockOrdered, int quantity)
        : base(orderID, stockOrdered, quantity) 
        {
            base.OrderType = "Short";
        }

        public override Trade MakeTrade()
        {
            BuyTrade trade = new BuyTrade(base.StockOrdered, base.Quantity, base.Value, base.TotalValue);
            trade.TradeType = base.OrderType;
            return trade;
        }
    }
}
