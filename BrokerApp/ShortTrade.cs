using System;

namespace BrokerApp
{
    public class ShortTrade: Trade
    {
        public ShortTrade(Stock stock, int quantity, double buyPrice, double TotalBuyValue)
        : base(stock, quantity, buyPrice, TotalBuyValue)
        {
            base.TradeType = "Short";
        }

        public override void ExecuteTrade()
        {
            throw new NotImplementedException();
        }
    }
}
