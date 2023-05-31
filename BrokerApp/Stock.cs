using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerApp
{
    public class Stock
    {
        private int _id;
        private string _name;
        private string _tickerSymbol;
        private double _currentValue;

        public Stock(int id, string name, string tickerSymbol, double currentValue)
        {
            _id = id;
            _name = name;
            _tickerSymbol = tickerSymbol;
            _currentValue = currentValue;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string TickerSymbol
        {
            get { return _tickerSymbol; }
            set { _tickerSymbol = value; }
        }

        public double Value
        {
            get { return _currentValue; }
            set { _currentValue = value; }
        }
    }
}
