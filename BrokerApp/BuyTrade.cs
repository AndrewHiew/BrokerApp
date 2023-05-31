using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerApp
{
    public class BuyTrade: Trade
    {
        public BuyTrade(Stock stock, int quantity, double buyPrice, double TotalBuyValue)
        : base(stock, quantity, buyPrice, TotalBuyValue) 
        {
            base.TradeType = "Buy";
        }

        public override void ExecuteTrade()
        {
            throw new NotImplementedException();
        }
    }
}
