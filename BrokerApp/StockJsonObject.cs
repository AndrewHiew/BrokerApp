namespace BrokerApp
{
    public class StockJsonObject
    {
        public class Data
        {
            public double currentPrice { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public string message { get; set; }
            public int status { get; set; }
        }
    }
}
