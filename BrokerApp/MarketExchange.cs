using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BrokerApp
{
    public class MarketExchange
    {
        private string _marketName;
        private List<Stock> _stocks;
        private Portfolio _portfolio;

        public MarketExchange(string marketName, Portfolio portfolio)
        {
            _marketName = marketName;
            _stocks = new List<Stock>();
            _portfolio = portfolio;
        }

        public List<Stock> GetStocks()
        {
            return _stocks;
        }

        public void AddStock(Stock stock)
        {
            _stocks.Add(stock);
        }

        public void DeleteStock(Stock stock)
        {
            _stocks.Remove(stock);
        }
        
        /// <summary>
        /// Add Order Object into Portfolio
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            _portfolio.AddOrder(order);
        }

        public void AddTrade(Trade trade)
        {
            _portfolio.AddTrade(trade);
        }

        public void AddOwnedStocks(OwnedStocks ownedStocks)
        {
            _portfolio.AddOwnedStocks(ownedStocks);
        }

        public Portfolio GetPortfolio()
        {
            return _portfolio;
        }

        public string Marketname
        {
            get { return _marketName; }
            set { _marketName = value; }
        }
    }
}
