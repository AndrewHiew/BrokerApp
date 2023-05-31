using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Property Field for DB
        public List<StockDB> DataBaseStockContent { get; private set; }
        public List<UserDB> DataBaseUserContent { get; private set; }

        //Create DB when when starting up the program
        protected override void OnStartup(StartupEventArgs e)
        {
            //Create DB If it is not present in the root folder
            DatabaseFacade facade = new DatabaseFacade(new DataContext());
            facade.EnsureCreated();

            //Insert Data into DB Only if This Database does not exists
            using (DataContext context = new DataContext())
            {
                //Build MarketPlace and User Object <<MOCK>>
                Portfolio portfolio = new Portfolio(0, 0);
                MarketExchange marketExchange = new MarketExchange("NYSE", portfolio);
                User user1 = new User("Andrew", "andrewhiew", marketExchange);
                User user2 = new User("Ariel", "qwerty", marketExchange);

                //Mock Data
                Stock stock1 = new Stock(1, "Apple Inc", "AAPL", 170);
                Stock stock2 = new Stock(2, "Tesla", "TSLA", 161);
                Stock stock3 = new Stock(3, "Palantir", "PLTR", 11.5);
                Stock stock4 = new Stock(4, "Microsoft", "MSFT", 318);
                Stock stock5 = new Stock(5, "Meta Platform", "META", 245.8);
                Stock stock6 = new Stock(6, "Alibaba Group Holding", "BABA", 80);
                Stock stock7 = new Stock(7, "Amazon Inc.", "AMZN", 116);
                Stock stock8 = new Stock(8, "Netflix", "NFLX", 365);

                //Add Base Stocks Into DB
                DataBaseStockContent = context.Stock.ToList();
                if (DataBaseStockContent == null || DataBaseStockContent.Count == 0)
                {
                    context.Stock.Add(new StockDB() { Name = stock1.Name, TickerSymbol = stock1.TickerSymbol, CurrentValue = stock1.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock2.Name, TickerSymbol = stock2.TickerSymbol, CurrentValue = stock2.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock3.Name, TickerSymbol = stock3.TickerSymbol, CurrentValue = stock3.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock4.Name, TickerSymbol = stock4.TickerSymbol, CurrentValue = stock4.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock5.Name, TickerSymbol = stock5.TickerSymbol, CurrentValue = stock5.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock6.Name, TickerSymbol = stock6.TickerSymbol, CurrentValue = stock6.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock7.Name, TickerSymbol = stock7.TickerSymbol, CurrentValue = stock7.Value });
                    context.SaveChanges();
                    context.Stock.Add(new StockDB() { Name = stock8.Name, TickerSymbol = stock8.TickerSymbol, CurrentValue = stock8.Value });
                    context.SaveChanges();
                }

                //Default Users
                DataBaseUserContent = context.Users.ToList();
                if (DataBaseUserContent == null || DataBaseUserContent.Count == 0)
                {
                    context.Users.Add(new UserDB()
                    {
                        Name = user1.Name,
                        Password = user1.Password,
                        AssetsValue = user1.GetMarketExchange().GetPortfolio().AssetsValue,
                        CurrentBalance = user1.GetMarketExchange().GetPortfolio().CurrentBalance
                    });
                    context.SaveChanges();

                    context.Users.Add(new UserDB()
                    {
                        Name = user2.Name,
                        Password = user2.Password,
                        AssetsValue = user2.GetMarketExchange().GetPortfolio().AssetsValue,
                        CurrentBalance = user2.GetMarketExchange().GetPortfolio().CurrentBalance
                    });
                    context.SaveChanges();
                }
            }

            //UPDATE APPLE STOCK
            //this.UpdateLiveStockAPPL();

            //UPDATE ALL STOCK PRICE
            this.UpdateLiveStock();

            //Build the and run the Login Window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        /// <summary>
        /// UPDATE ALL STOCK PRICE 
        /// </summary>
        /// <returns></returns>
        private async Task UpdateLiveStock()
        {
            using (DataContext context = new DataContext())
            {
                // Build YahooFinanceAPI Object
                YahooFinanceAPI yahooFinanceAPI = new YahooFinanceAPI();

                // Retrieve Stock Data from Database
                List<StockDB> stockList = await context.Stock.ToListAsync();

                foreach (StockDB stockDB in stockList)
                {
                    double stockValue = await yahooFinanceAPI.GetStockCurrentValueAsync(stockDB.TickerSymbol);
                    if (stockValue != 0)
                    {
                        stockDB.CurrentValue = Math.Round(stockValue, 2);
                    }
                }

                // Save Details Into DataBase
                await context.SaveChangesAsync();
            }
        }


        /// <summary>
        /// FOR TESTING, UPDATE ONLY APPL STOCK
        /// </summary>
        private void UpdateLiveStockAPPL()
        {
            using (DataContext context = new DataContext())
            {
                //Build YahooFinanceAPI Object
                YahooFinanceAPI yahooFinanceAPI = new YahooFinanceAPI();

                //Retrieve Stock Data from Database
                DataBaseStockContent = context.Stock.ToList();

                foreach (StockDB stockDB in DataBaseStockContent)
                {
                    if (stockDB.TickerSymbol.Equals("AAPL"))
                    {
                        //double stockValue = yahooFinanceAPI.GetStockCurrentValue("AAPL");
                        //stockDB.CurrentValue = stockValue;
                    }
                }

                //Save Details Into DataBase
                context.SaveChanges();
            }
        }
    }
}
