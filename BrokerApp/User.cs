namespace BrokerApp
{
    public class User
    {
        private int _id;
        private string _name;
        private string _password;
        private MarketExchange _marketExchange;

        //Constructor Overload
        public User(string name, string password, MarketExchange marketExchange)
        {
            _name = name;
            _password = password;
            _marketExchange = marketExchange;
        }

        public User(int id, string name, string password, MarketExchange marketExchange)
        {
            _id = id;
            _name = name;
            _password = password;
            _marketExchange = marketExchange;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public MarketExchange GetMarketExchange()
        {
            return _marketExchange;
        }

        public void AddCredit(int amount)
        {
            _marketExchange.GetPortfolio().UserAddCredit(amount);
        }
    }
}
