using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for ViewOwnedStockWindow.xaml
    /// </summary>
    public partial class ViewOwnedStockWindow : Window
    {
        private User _user;
        private OwnedStocks _ownedstock;
        public ViewOwnedStockWindow(User user, OwnedStocks ownedStock)
        {
            InitializeComponent();
            _user = user;
            _ownedstock = ownedStock;

            //Set Field Values
            this.SetFieldValues();

            //Click Event
            BackBtn.Click += (object sender, RoutedEventArgs e) => { BackBtn_Click(sender, e); };
            SellBtn.Click += (object sender, RoutedEventArgs e) => { SellBtn_Click(sender, e); };
        }

        /// <summary>
        /// Back button click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageAssets manageAssets = new ManageAssets(_user);
            this.Close();
            manageAssets.Show();
        }

        /// <summary>
        /// SellBtn click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SellBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(SellQuantityField.Text.Equals("")))
            {
                int sellQuantity = int.Parse(SellQuantityField.Text);

                if (sellQuantity < 1)
                {
                    MessageBox.Show("Please enter a valid number.");
                }

                else
                {
                    this.ExecuteSellOperation(sellQuantity);
                }
            }
        }

        /// <summary>
        /// Main method to executes sell stock operation
        /// </summary>
        /// <param name="sellQuantity"></param>
        private void ExecuteSellOperation(int sellQuantity)
        {
            _ownedstock.Quantity -= sellQuantity;
            if (_ownedstock.Quantity.Equals(0))
            {
                _user.GetMarketExchange().GetPortfolio().RemoveOwnedStocks(_ownedstock);
                this.CalculateProfit(sellQuantity);
                this.DeleteOwnedStockDB(sellQuantity);

                MessageBox.Show("All Stock Sold.");
                ManageAssets manageAssets = new ManageAssets(_user);
                this.Close();
                manageAssets.Show();
            }
            else
            {
                this.CalculateProfit(sellQuantity);
                this.SellStock(sellQuantity);
            }
        }

        /// <summary>
        /// Method to delete/remove ownedStock in Database
        /// </summary>
        /// <param name="sellQuantity"></param>
        private void DeleteOwnedStockDB(int sellQuantity)
        {
            using (DataContext context = new DataContext())
            {
                List<UserOwnedStockDB> ownedStockListDB = new List<UserOwnedStockDB>();

                //Build SellTrade object and add into DB
                this.BuildAndAddSellTrade(sellQuantity, context);

                //Delete UserOwnedStock from DB
                ownedStockListDB = context.UserOwnedStock.ToList();
                foreach (UserOwnedStockDB ownedStock in context.UserOwnedStock.ToList())
                {
                    if (ownedStock.UserID.Equals(_user.ID) && ownedStock.StockID.Equals(_ownedstock.Stock.Id) && ownedStock.OwnedType.Equals(_ownedstock.OwnedType) && ownedStock.Id.Equals(_ownedstock.ID))
                    {
                        context.UserOwnedStock.Remove(ownedStock);
                    }
                }

                //Save Changes
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Method that handles SellStockChecker and BuildAndAddSellTrade method
        /// </summary>
        /// <param name="sellQuantity"></param>
        private void SellStock(int sellQuantity)
        {
            using (DataContext context = new DataContext())
            {
                List<UserOwnedStockDB> ownedStockListDB = new List<UserOwnedStockDB>();

                //Sell stock and Update the DB
                ownedStockListDB = context.UserOwnedStock.ToList();
                foreach (UserOwnedStockDB ownedStock in context.UserOwnedStock.ToList())
                {
                    if (ownedStock.UserID.Equals(_user.ID) && ownedStock.StockID.Equals(_ownedstock.Stock.Id) && ownedStock.OwnedType.Equals(_ownedstock.OwnedType) && ownedStock.Id.Equals(_ownedstock.ID))
                    {
                        int checker = ownedStock.Quantity - sellQuantity;
                        this.SellStockChecker(checker, sellQuantity, ownedStock);
                        this.BuildAndAddSellTrade(sellQuantity, context);
                        break;
                    }
                }

                //Save Changes
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Method to check sell stock quantity
        /// </summary>
        /// <param name="checker"></param>
        /// <param name="sellQuantity"></param>
        /// <param name="ownedStock"></param>
        private void SellStockChecker(int checker, int sellQuantity, UserOwnedStockDB ownedStock)
        {
            if (checker < 1)
            {
                MessageBox.Show("You don't have that many stock.");
            }
            else
            {
                ownedStock.Quantity -= sellQuantity;
                MessageBox.Show(sellQuantity + " Number of Stocks Sold.");

                //Recalculate OwnedStock object totalvalue
                _ownedstock.CalculateValue();

                //Reset Field Values
                this.ResetFieldValues();
                this.SetFieldValues();
            }
        }

        /// <summary>
        /// Method to build sell trade and add it into trade history
        /// </summary>
        /// <param name="sellQuantity"></param>
        /// <param name="context"></param>
        private void BuildAndAddSellTrade(int sellQuantity, DataContext context)
        {
            double totalValue = _ownedstock.Stock.Value * sellQuantity;
            totalValue = Math.Round(totalValue, 2);
            SellTrade sellTrade = new SellTrade(_ownedstock.Stock, sellQuantity, _ownedstock.Stock.Value, totalValue);
            TradeDB tradeDB = new TradeDB
            {
                TradeType = sellTrade.TradeType,
                StockID = sellTrade.StockID,
                UserID = _user.ID,
                Quantity = sellTrade.Quantity,
                BuyPrice = sellTrade.BuyPrice,
                TotalBuyValue = sellTrade.TotalBuyValue,
                UserCurrentBalance = _user.GetMarketExchange().GetPortfolio().CurrentBalance,
                UserTotalBalance = _user.GetMarketExchange().GetPortfolio().TotalBalance
            };
            context.Trade.Add(tradeDB);
            context.SaveChanges();
        }

        /// <summary>
        /// Method to calculate profit based on ownedType (Short or buy)
        /// </summary>
        /// <param name="sellQuantity"></param>
        private void CalculateProfit(int sellQuantity)
        {
            double reimburseBalance = _ownedstock.Stock.Value * sellQuantity;
            reimburseBalance = Math.Round(reimburseBalance, 2);

            if (_ownedstock.OwnedType.Equals("Buy"))
            {
                //Add profit into current balance
                _user.GetMarketExchange().GetPortfolio().CurrentBalance += reimburseBalance;
                _user.GetMarketExchange().GetPortfolio().AssetsValue -= reimburseBalance;
                _user.GetMarketExchange().GetPortfolio().CalculateTotalBalanceWithAssets();
            }
            else
            {
                //<<TODO SHORT STOCK LOGIC>> ADD TOTALBUYVALUE PARAMETER TO OWNEDSTOCK DB
                //Add profit into current balance
                _user.GetMarketExchange().GetPortfolio().CurrentBalance += reimburseBalance;
                _user.GetMarketExchange().GetPortfolio().AssetsValue -= reimburseBalance;
                _user.GetMarketExchange().GetPortfolio().CalculateTotalBalanceWithAssets();
            }

            //Update UserDB
            this.UpdateUserDB();
        }

        /// <summary>
        /// Method to update currentBalance in UserDB
        /// </summary>
        private void UpdateUserDB()
        {
            using (DataContext context = new DataContext())
            {
                UserDB userDB = context.Users.FirstOrDefault(user => user.Id == _user.ID);

                if (userDB != null)
                {
                    userDB.CurrentBalance = _user.GetMarketExchange().GetPortfolio().CurrentBalance;
                    userDB.AssetsValue = _user.GetMarketExchange().GetPortfolio().AssetsValue;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Method to set the Textblock values
        /// </summary>
        private void SetFieldValues()
        {
            OwnedType.Text = _ownedstock.OwnedType;
            StockTickerSymbolBox.Text = _ownedstock.TickerSymbol;
            StockNameBox.Text = _ownedstock.Name;
            StockValueBox.Text = Math.Round(_ownedstock.Value, 2).ToString();
            StockQuantityBox.Text = _ownedstock.Quantity.ToString();
            TotalValueBox.Text = Math.Round(_ownedstock.TotalValue, 2).ToString();
        }

        /// <summary>
        /// Method to reset the Textblock values
        /// </summary>
        private void ResetFieldValues()
        {
            OwnedType.Text = string.Empty;
            StockTickerSymbolBox.Text = string.Empty;
            StockNameBox.Text = string.Empty;
            StockValueBox.Text = string.Empty;
            StockQuantityBox.Text = string.Empty;
            TotalValueBox.Text = string.Empty;
        }

        /// <summary>
        /// Method that restrict datatypes in inputfield (UI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9\.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
