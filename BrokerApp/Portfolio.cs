using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerApp
{
    public class Portfolio
    {
        private List<Trade> _tradeHistory;
        private List<OwnedStocks> _ownedStocks;
        private List<Order> _orderList;
        private double _currentBalance;
        private double _assetsValue;
        private double _totalBalance;

        public Portfolio(double currentBalance, double AssetsValue)
        {
            _currentBalance = currentBalance;
            _assetsValue = AssetsValue;

            //Instantiate Lists
            _tradeHistory = new List<Trade>();
            _ownedStocks = new List<OwnedStocks>();
            _orderList = new List<Order>();

            //Calculate TotalBalance
            this.CalculateTotalBalanceWithAssets();
            this.RoundUpValues();
        }

        public void AddOrder(Order order)
        {
            _orderList.Add(order);
        }

        public void RemoveOrder(Order order)
        {
            _orderList.Remove(order);
        }

        public void AddOwnedStocks(OwnedStocks ownedStock)
        {
            _ownedStocks.Add(ownedStock);
        }

        public void RemoveOwnedStocks(OwnedStocks ownedStocks)
        {
            _ownedStocks.Remove(ownedStocks);
        }
        public List<OwnedStocks> GetOwnedStockList() { return _ownedStocks; }

        public List<Order> GetOrderList() { return _orderList; }

        public List<Trade> GetTradeHistory()
        {
            return _tradeHistory;
        }

        public void AddTrade(Trade trade)
        {
            _tradeHistory.Add(trade);
        }

        public double CurrentBalance
        {
            get { return _currentBalance; }
            set { _currentBalance = value; }
        }

        public double AssetsValue
        {
            get { return _assetsValue; }
            set { _assetsValue = value; }
        }

        public double TotalBalance
        {
            get { return _totalBalance; }
            set { _totalBalance = value; }
        }

        public void CalculateTotalBalanceWithAssets()
        {
            _totalBalance = _assetsValue + _currentBalance;
            this.RoundUpValues();
        }

        public void UserAddCredit(int amount)
        {
            _currentBalance += amount;
            this.CalculateTotalBalanceWithAssets();
        }

        public void RoundUpValues()
        {
            _currentBalance = Math.Round(_currentBalance, 2);
            _assetsValue = Math.Round(_assetsValue, 2);
            _totalBalance = Math.Round(_totalBalance, 2);
        }
    }
}
