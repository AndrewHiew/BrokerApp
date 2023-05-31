using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace BrokerApp
{
    public class YahooFinanceAPI
    {
        public async Task<double> GetStockCurrentValueAsync(string symbol)
        {
            string stockSymbol = "symbol=" + symbol;
            var client = new RestClient("https://yfinance-stock-market-data.p.rapidapi.com/stock-info");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("X-RapidAPI-Key", "05509ceb8emsh8051255d0e95433p1ca00ejsn207716eaa2b2");
            request.AddHeader("X-RapidAPI-Host", "yfinance-stock-market-data.p.rapidapi.com");
            request.AddParameter("application/x-www-form-urlencoded", stockSymbol, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);

            StockJsonObject.Root value = JsonConvert.DeserializeObject<StockJsonObject.Root>(response.Content);

            if (response.IsSuccessful)
            {
                var content = response.Content;
                return value.data.currentPrice;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return 0;
            }
        }
    }
}
 