using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Click Event
            LoginSubmitBtn.Click += (object sender, RoutedEventArgs e) => { SubmitButton_Click(sender, e); };
        }
        public ObservableCollection<Stock> Stocks { get; private set; }

        /// <summary>
        /// SubmitButton Click Event Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            //Using statement to close the DataContext Object after it serves its purpose
            using (DataContext context = new DataContext())
            {
                GrantAccess(context);
            }
            PasswordField.Clear();
        }

        /// <summary>
        /// Method that verify user credentials. And more
        /// </summary>
        /// <param name="context"></param>
        private void GrantAccess(DataContext context)
        {
            //Search DB for matching Results
            UserDB user = context.Users.FirstOrDefault(u => u.Name.Equals(UsernameField.Text.Trim()) && u.Password.Equals(PasswordField.Password.Trim()));

            //Ready List to add Objects into it from Database
            List<Stock> stockList = new List<Stock>();

            //Get Data from Database
            List<StockDB> stockDBList = context.Stock.ToList();

            //Add Stock Object from DB into MarketExchange (Part 1)
            foreach (StockDB stockDB in stockDBList)
            {
                Stock stock = new Stock(stockDB.Id, stockDB.Name, stockDB.TickerSymbol, stockDB.CurrentValue);
                stockList.Add(stock);
            }

            //If Result is found
            if (user != null)
            {
                ErrorMessageBox.Text = "";

                //Build User Object
                Portfolio setPortfolio = new Portfolio(user.CurrentBalance, user.AssetsValue);
                MarketExchange marketExchange = new MarketExchange("$NYSE", setPortfolio);
                User setUser = new User(user.Id, user.Name, user.Password, marketExchange);

                //Adding Stocks into MarketExchange
                this.AddStockIntoMarketExchange(stockList, marketExchange);

                //Update User TotalAssets Value
                List<UserOwnedStockDB> ownedStockList = context.UserOwnedStock.Where(ownedStockDB => ownedStockDB.UserID.Equals(user.Id)).ToList();
                this.UpdateUserAssetValue(ownedStockList, stockList, setUser, user, context);
                
                //Assign User into Main Page
                UserMainPage userMain = new UserMainPage(setUser);

                //Close Login Window and Reveal User Main Window
                this.Close();
                userMain.Show();
            }

            //If Results is not found, Display an error message
            else
            {
                ErrorMessageBox.Text = "Invalid Credentials";
            }
        }

        /// <summary>
        /// Method to Add Stock object Into MarketExchange
        /// </summary>
        /// <param name="stockList"></param>
        /// <param name="marketExchange"></param>
        private void AddStockIntoMarketExchange(List<Stock> stockList, MarketExchange marketExchange)
        {
            foreach (Stock stock in stockList)
            {
                marketExchange.AddStock(stock);
            }
        }

        /// <summary>
        /// Update AssetsValue according to Updated Stock price
        /// </summary>
        /// <param name="userOwnedStockDBList"></param>
        /// <param name="setUser"></param>
        private void UpdateUserAssetValue(List<UserOwnedStockDB> ownedStockList, List<Stock> stockList, User setUser, UserDB user, DataContext context)
        {
            double totalAssetsValue = 0;

            //Calculate Assets Value
            foreach (UserOwnedStockDB ownedStocksDB in ownedStockList)
            {
                foreach (Stock stock in stockList)
                {
                    if (ownedStocksDB.StockID.Equals(stock.Id))
                    {
                        totalAssetsValue += stock.Value * ownedStocksDB.Quantity;
                    }
                }
            }

            totalAssetsValue = Math.Round(totalAssetsValue, 2);
            setUser.GetMarketExchange().GetPortfolio().AssetsValue = totalAssetsValue;
            setUser.GetMarketExchange().GetPortfolio().CalculateTotalBalanceWithAssets();
            user.AssetsValue = totalAssetsValue;
            context.SaveChanges();
        }
    }
}
