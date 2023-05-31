using System;

namespace BrokerApp
{
    public abstract class Trade
    {
        private Stock _stock;
        private string _tradeType;
        private int _quantity;
        private double _buyPrice;
        private double _totalBuyValue;

        public Trade(Stock stock, int quantity, double buyPrice, double TotalBuyValue)
        {
            _stock = stock;
            _quantity = quantity;
            _buyPrice = buyPrice;
            _totalBuyValue = TotalBuyValue;
            _totalBuyValue = Math.Round(_totalBuyValue, 2);
        }

        public Stock Stock
        {
            get { return _stock; }
            set { _stock = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public int StockID
        {
            get { return _stock.Id; }
            set { _stock.Id = value; }
        }
        public string TradeType
        {
            get { return _tradeType; }
            set { _tradeType = value; }
        }
        public string TickerSymbol
        {
            get { return _stock.TickerSymbol; }
            set { _stock.TickerSymbol = value; }
        }

        public string Name
        {
            get { return _stock.Name; }
            set { _stock.Name = value; }
        }

        public double BuyPrice
        {
            get { return _buyPrice; }
            set { _buyPrice = value; }
        }

        public double TotalBuyValue
        {
            get { return _totalBuyValue; }
            set { _totalBuyValue = value; }
        }

        public abstract void ExecuteTrade();
    }
}
