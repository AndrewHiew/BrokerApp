using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerApp
{
    public class OwnedStocks
    {
        private int _id;
        private Stock _stock;
        private int _quantity;
        private double _totalValue;
        private string _ownedType;
        private double _stockBoughtValue;

        /// <summary>
        /// Normal constructor used in creating owned stock object
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="quantity"></param>
        /// <param name="ownedType"></param>
        public OwnedStocks(int id, Stock stock, int quantity, string ownedType) 
        {
            _id = id;
            _stock = stock;
            _quantity = quantity;
            _ownedType = ownedType;
            _stockBoughtValue = stock.Value;
            this.CalculateValue();
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stock"></param>
        /// <param name="quantity"></param>
        /// <param name="ownedType"></param>
        public OwnedStocks(Stock stock, int quantity, string ownedType)
        {
            _stock = stock;
            _quantity = quantity;
            _ownedType = ownedType;
            _stockBoughtValue = stock.Value;
            this.CalculateValue();
        }

        /// <summary>
        /// Constructor overload that takes Stock object, int quantity, string ownedType and double stockBoughtValue. 
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="quantity"></param>
        /// <param name="ownedType"></param>
        /// <param name="stockBoughtValue"></param>
        public OwnedStocks(Stock stock, int quantity, string ownedType, double stockBoughtValue) 
        {
            _stock = stock;
            _quantity = quantity;
            _ownedType = ownedType;
            _stockBoughtValue = stockBoughtValue;
            this.CalculateValue();
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }    
        }

        public Stock Stock
        { 
            get { return _stock; } 
            set { _stock = value; }
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

        public double Value
        {
            get { return _stock.Value; }
            set { _stock.Value = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public double TotalValue
        {
            get { return _totalValue; }
            set { _totalValue = value; }
        }

        public string OwnedType
        {
            get { return _ownedType; }
            set { _ownedType = value; }
        }

        public double StockBoughtValue
        {
            get { return _stockBoughtValue; }
            set { _stockBoughtValue = value; }
        }

        public Stock GetStock() { return _stock; }

        public void CalculateValue()
        {
            _totalValue = _stock.Value * _quantity;
            _totalValue = Math.Round(_totalValue, 2);
        }
    }
}
