using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for UserMainPage.xaml
    /// </summary>
    public partial class UserMainPage : Window
    {
        private User _user;
        private Portfolio _portfolio;

        public UserMainPage(User user)
        {
            InitializeComponent();
            _user = user;
            _portfolio = _user.GetMarketExchange().GetPortfolio();

            NameField.Text = _user.Name;

            //Click Event
            LogoutBtn.Click += (object sender, RoutedEventArgs e) => { LogoutBtn_Click(sender, e); };
            DepositBtn.Click += (object sender, RoutedEventArgs e) => { DepositBtn_Click(sender, e); };
            WithdrawBtn.Click += (object sender, RoutedEventArgs e) => { WithdrawBtn_Click(sender, e); };
            ManageAssetsBtn.Click += (object sender, RoutedEventArgs e) => { ManageAssetsBtn_Click(sender, e); };
            checkPerformanceBtn.Click += (object sender, RoutedEventArgs e) => { checkPerformanceBtn_Click(sender, e); };

            //Adding Stock Objects into the List
            this.AddStocksIntoListBox();

            //Binding TotalBalance and CurrentBalance
            this.SetBalanceText(_portfolio);
        }
        public ObservableCollection<Stock> Stocks { get; private set; }

        /// <summary>
        /// LogoutButton click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void checkPerformanceBtn_Click(object sender, RoutedEventArgs e)
        {
            PerformanceChart p = new PerformanceChart(_user);
            p.Show();
            this.Close();
        }

        /// <summary>
        /// DepositButton click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DepositBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DepositAmountBox.Text))
            {
                double depositAmount = double.Parse(DepositAmountBox.Text);
                if (!(depositAmount < 0))
                {
                    using (DataContext context = new DataContext())
                    {
                        //Update Database
                        int selectedUserID = _user.ID;
                        UserDB userDB = context.Users.Find(selectedUserID);
                        userDB.CurrentBalance += depositAmount;

                        //Round up number to 2 decimal place
                        userDB.CurrentBalance = Math.Round(userDB.CurrentBalance, 2);

                        //Update Portfolio Object
                        _portfolio.CurrentBalance = userDB.CurrentBalance;
                        _portfolio.CalculateTotalBalanceWithAssets();

                        //Save Changes
                        context.SaveChanges();
                    }

                    MessageBox.Show("Funds Successfully Deposited");
                    DepositAmountBox.Text = string.Empty;
                    WithdrawAmountBox.Text = string.Empty;

                    //<<EXPERIMENT>> HardSet the textblock value
                    this.RefreshBalanceText();

                    //Reupdate Bound Data...
                    BindingExpression bindingExpression = TotalBalanceBox.GetBindingExpression(TextBlock.TextProperty);
                    bindingExpression?.UpdateSource();
                }
                else
                {
                    MessageBox.Show("Please Enter a positive Integer.");
                }
            }
            else
            {
                MessageBox.Show("Please Enter an Amount.");
            }
        }

        /// <summary>
        /// WithdrawButton click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WithdrawBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(WithdrawAmountBox.Text))
            {
                double depositAmount = double.Parse(WithdrawAmountBox.Text);
                if (!(depositAmount < 0))
                {
                    this.CheckWithdrawAmount(depositAmount);
                }
                else
                {
                    MessageBox.Show("Please Enter a positive Integer.");
                }
            }
            else
            {
                MessageBox.Show("Please Enter an Amount.");
            }
        }

        /// <summary>
        /// ManageAssetsbutton click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageAssetsBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageAssets manageAssets = new ManageAssets(_user);
            this.Close();
            manageAssets.Show();
        }

        /// <summary>
        /// Method to add stock object into StockListBox (UI)
        /// </summary>
        public void AddStocksIntoListBox()
        {
            List<Stock> StockList = _user.GetMarketExchange().GetStocks();

            Stocks = new ObservableCollection<Stock>(StockList);
            StockListBox.ItemsSource = Stocks;        
        }

        /// <summary>
        /// Method that defines StockListBox behaviour and click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StockListBox.SelectedItem != null)
            {
                // Retrieve the selected item and cast it to your Stock class
                Stock selectedStock = StockListBox.SelectedItem as Stock;

                // Access the properties of the selected stock
                int id = selectedStock.Id;
                string name = selectedStock.Name;
                string tickerSymbol = selectedStock.TickerSymbol;
                double value = selectedStock.Value;

                // Do something with the selected stock data
                StockDetailsPage stockDetailsPage = new StockDetailsPage(_user, selectedStock);
                stockDetailsPage.Show();
                this.Close();

                // Clear the selection
                StockListBox.SelectedItem = null;
            }
        }

        private void CheckWithdrawAmount(double depositAmount)
        {
            if (!(depositAmount > _portfolio.CurrentBalance))
            {
                using (DataContext context = new DataContext())
                {
                    //Update Database
                    int selectedUserID = _user.ID;
                    UserDB userDB = context.Users.Find(selectedUserID);
                    userDB.CurrentBalance -= depositAmount;

                    //Round up number to 2 decimal place
                    userDB.CurrentBalance = Math.Round(userDB.CurrentBalance, 2);

                    //Update Portfolio Object
                    _portfolio.CurrentBalance = userDB.CurrentBalance;
                    _portfolio.CalculateTotalBalanceWithAssets();

                    //Save Changes
                    context.SaveChanges();
                }

                MessageBox.Show("Funds Successfully Withdrawn");
                DepositAmountBox.Text = string.Empty;
                WithdrawAmountBox.Text = string.Empty;

                //<<EXPERIMENT>> HardSet the textblock value
                this.RefreshBalanceText();

                //Reupdate Bound Data...
                BindingExpression bindingExpression = TotalBalanceBox.GetBindingExpression(TextBlock.TextProperty);
                bindingExpression?.UpdateSource();
            }
            else
            {
                MessageBox.Show("You dont have enough credit to withdraw.");
            }
        }

        /// <summary>
        /// Method that binds UI DataContext to portfolio object
        /// </summary>
        /// <param name="portfolio"></param>
        private void SetBalanceText(Portfolio portfolio)
        {
            TotalBalanceBox.DataContext = portfolio;
            CurrentBalanceBox.DataContext = portfolio;
        }

        /// <summary>
        /// Method that refreshes bound data (Balance TextBlock UI)
        /// </summary>
        private void RefreshBalanceText()
        {
            CurrentBalanceBox.Text = _portfolio.CurrentBalance.ToString();
            TotalBalanceBox.Text = _portfolio.TotalBalance.ToString();
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
