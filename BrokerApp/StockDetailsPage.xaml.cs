using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for StockDetailsPage.xaml
    /// </summary>
    public partial class StockDetailsPage : Window
    {
        private User _user;
        private Stock _selectedStock;
        public StockDetailsPage(User user, Stock selectedStock)
        {
            InitializeComponent();
            _user = user;
            _selectedStock = selectedStock;

            this.SetStockFieldValue();

            //Click Event
            BackBtn.Click += (object sender, RoutedEventArgs e) => { BackBtn_Click(sender, e); };
            BuyOrderBtn.Click += (object sender, RoutedEventArgs e) => { BuyOrderBtn_Click(sender, e); };
            ShortOrderBtn.Click += (object sender, RoutedEventArgs e) => { ShortOrderBtn_Click(sender, e); };
            StockLinkBtn.Click += (object sender, RoutedEventArgs e) => { StockLinkBtn_Click(sender, e);  };
        }

        /// <summary>
        /// BackButton click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.GoBackToUserMain();
        }

        private void StockLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

            if (_selectedStock.TickerSymbol.Equals("AAPL"))
            {
                string url = "https://finance.yahoo.com/quote/AAPL?p=AAPL&.tsrc=fin-srch";

                // Start the Chrome process with the URL as a command-line argument
                Process.Start(new ProcessStartInfo(chromePath, url));
            }
        }

        private void GoBackToUserMain()
        {
            UserMainPage userMainPage = new UserMainPage(_user);
            userMainPage.Show();
            this.Close();
        }

        private void BuyOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(StockQuantityBox.Text) != 0)
            {
                this.BuyCheckQuantityField();
                StockQuantityBox.Text = "";
            }
            else
            {
                StockQuantityBox.Text = "";
                MessageBox.Show($"Please Enter Quantity");
            }
        }

        private void ShortOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(StockQuantityBox.Text) != 0)
            {
                this.ShortCheckQuantityField();
                StockQuantityBox.Text = "";
            }
            else
            {
                StockQuantityBox.Text = "";
                MessageBox.Show($"Please Enter Quantity");
            }      
        }

        /// <summary>
        /// Input Validation for BuyOrder
        /// </summary>
        private void BuyCheckQuantityField()
        {
            if (int.Parse(StockQuantityBox.Text) < 0)
            {
                StockQuantityBox.Text = "";
                MessageBox.Show($"Please Enter A Positive Integer");
            }
            else
            {
                //Add New Order details into DB
                OrderDB order = new OrderDB
                {
                    OrderType = "Buy",
                    StockID = _selectedStock.Id,
                    UserID = _user.ID,
                    Quantity = int.Parse(StockQuantityBox.Text)
                };
                this.AddOrderDB(order);
                MessageBox.Show($"Order Added");

                //Update Current Object
                BuyOrder newOrder = new BuyOrder(this.Testing(), _selectedStock, int.Parse(StockQuantityBox.Text));
                _user.GetMarketExchange().GetPortfolio().AddOrder(newOrder);

                this.GoBackToUserMain();
            }
        }

        /// <summary>
        /// Input Validation for Short Order
        /// </summary>
        private void ShortCheckQuantityField()
        {
            if (int.Parse(StockQuantityBox.Text) < 0)
            {
                StockQuantityBox.Text = "";
                MessageBox.Show($"Please Enter A Positive Integer");
            }
            else
            {
                //Add New Order details into DB
                OrderDB order = new OrderDB
                {
                    OrderType = "Short",
                    StockID = _selectedStock.Id,
                    UserID = _user.ID,
                    Quantity = int.Parse(StockQuantityBox.Text)
                };
                this.AddOrderDB(order);
                MessageBox.Show($"Order Added");

                //Update Current Object
                ShortOrder newOrder = new ShortOrder(this.Testing(), _selectedStock, int.Parse(StockQuantityBox.Text));
                _user.GetMarketExchange().GetPortfolio().AddOrder(newOrder);

                this.GoBackToUserMain();
            }
        }

        /// <summary>
        /// Adding order details into Database
        /// </summary>
        /// <param name="order"></param>
        private void AddOrderDB(OrderDB order)
        {
            using (DataContext context = new DataContext())
            {
                context.Order.Add(order);
                context.SaveChanges();
            }
        }

        //<REMOVE> TEST METHOD
        private int Testing()
        {
            using (DataContext context = new DataContext())
            {
                // Retrieve the last row from the database table
                OrderDB lastOrder = context.Order.OrderByDescending(o => o.OrderID).FirstOrDefault();

                if (lastOrder != null)
                {
                    // Access the properties of the last order
                    int id = lastOrder.OrderID;
                    return id;
                }
                else
                {
                    // Handle the case where no orders exist in the table
                    MessageBox.Show("No orders found.");
                    return 1;
                }
            }
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

        /// <summary>
        /// Setting Stock Details field value
        /// </summary>
        private void SetStockFieldValue()
        {
            StockTickerSymbolBox.Text = _selectedStock.TickerSymbol;
            StockNameBox.Text = _selectedStock.Name;
            StockValueBox.Text = _selectedStock.Value.ToString();
        }
    }
}
