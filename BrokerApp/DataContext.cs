using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BrokerApp
{
    /// <summary>
    /// DataContext Class, reesponsible in connection to DataBase and query
    /// </summary>
    public class DataContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = BrokerAppData.db");
        }

        public DbSet<UserDB> Users { get; set; }
        public DbSet<StockDB> Stock { get; set; }
        public DbSet<OrderDB> Order { get; set; }
        public DbSet<TradeDB> Trade { get; set; }  
        public DbSet<UserOwnedStockDB> UserOwnedStock { get; set; }


        /// <summary>
        /// Method to stream real-time stock price from the market
        /// </summary>
        /// <returns></returns>
        public async Task UpdateLiveStock()
        {
            // Build YahooFinanceAPI Object
            YahooFinanceAPI yahooFinanceAPI = new YahooFinanceAPI();

            // Retrieve Stock Data from Database
            List<StockDB> stockList = await this.Stock.ToListAsync();

            foreach (StockDB stockDB in stockList)
            {
                double stockValue = await yahooFinanceAPI.GetStockCurrentValueAsync(stockDB.TickerSymbol);
                if (stockValue != 0)
                {
                    stockDB.CurrentValue = Math.Round(stockValue, 2);
                }
            }

            // Save Details Into DataBase
            await this.SaveChangesAsync();
        }
    }
}
