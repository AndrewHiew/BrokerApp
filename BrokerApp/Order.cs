using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerApp
{
    public abstract class Order
    {
        private int _orderID;
        private Stock _stockOrdered;
        private string _orderType;
        private int _quantity;
        private double _totalValue;

        public Order(int orderID, Stock stockOrdered, int quantity)
        {
            _orderID = orderID;
            _stockOrdered = stockOrdered;
            _quantity = quantity;
            this.CalculateAndSetValue();
        }

        public abstract Trade MakeTrade();

        public int OrderID
        {
            get { return _orderID; }
            set { _orderID = value; }
        }

        public Stock StockOrdered
        {
            get { return _stockOrdered; }
            set { _stockOrdered = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public int StockID 
        { 
            get { return _stockOrdered.Id; }
            set { _stockOrdered.Id = value; }
        }
        public string OrderType 
        { 
            get { return _orderType; }
            set { _orderType = value; }
        }
        public string TickerSymbol
        {
            get { return _stockOrdered.TickerSymbol; }
            set { _stockOrdered.TickerSymbol = value; }
        }

        public string Name
        {
            get { return _stockOrdered.Name; }
            set { _stockOrdered.Name = value;}
        }

        public double Value
        {
            get { return _stockOrdered.Value; }
            set { _stockOrdered.Value = value; }
        }

        public double TotalValue
        {
            get { return _totalValue; }
            set { _totalValue = value; }
        }

        public Stock GetOrderedStock()
        {
            return _stockOrdered;
        }

        private void CalculateAndSetValue()
        {
            _totalValue = _stockOrdered.Value * _quantity;
            _totalValue = Math.Round(_totalValue, 2);
        }
    }
}
