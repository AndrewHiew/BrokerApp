using OxyPlot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for ManageAssets.xaml
    /// </summary>
    public partial class ManageAssets : Window
    {
        private User _user;

        //ObservableCollection Data type to display the objects into List
        public ObservableCollection<OwnedStocks> OwnedStockColl { get; private set; }
        public ObservableCollection<Order> OrderColl { get; private set; }
        public ObservableCollection<Trade> TradeColl { get; private set; }

        /// <summary>
        /// ManageAssets Constructor
        /// </summary>
        /// <param name="user"></param>
        public ManageAssets(User user)
        {
            InitializeComponent();
            _user = user;

            //Set and Build Portfolio
            using (DataContext context = new DataContext()) { this.SetAndBuildPortfolioData(context); }
                
            //Add OwnedStocks into List
            this.AddOwnedStockDetailsIntoListBox();
            this.AddOrderIntoListBox();
            this.AddTradeIntoListBox();

            //Click Event
            BackBtn.Click += (object sender, RoutedEventArgs e) => { BackBtn_Click(sender, e); };
            SelectBtn.Click += (object sender, RoutedEventArgs e) => { SelectBtn_Click(sender, e); };
            OrderDeleteBtn.Click += (object sender, RoutedEventArgs e) => { OrderDeleteBtn_Click(sender, e); };
            OrderSelectBtn.Click += (object sender, RoutedEventArgs e) => { OrderSelectBtn_Click(sender, e); };
            PerformanceBtn.Click += (object sender, RoutedEventArgs e) => { PerformanceBtn_Click(sender, e); };
        }

        /// <summary>
        /// Back button click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            UserMainPage userMainPage = new UserMainPage(_user);
            this.Close();
            userMainPage.Show();
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            OwnedStocks ownedStock = OwnedStockListBox.SelectedItem as OwnedStocks;
            ViewOwnedStockWindow viewOwnedStockWindow = new ViewOwnedStockWindow(_user, ownedStock);
            this.Close();
            viewOwnedStockWindow.Show();
        }

        /// <summary>
        /// Order button click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrderListBox.SelectedItem != null)
            {
                using (DataContext context = new DataContext())
                {
                    Order selectedOrder = OrderListBox.SelectedItem as Order;
                    this.AddTradeIntoDBChecker(selectedOrder, context);
                }
            }
        }

        /// <summary>
        /// Order button click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrderListBox.SelectedItem != null)
            {
                using (DataContext context = new DataContext())
                {
                    Order selectedOrder = OrderListBox.SelectedItem as Order;

                    this.DeleteOrderFromDB(selectedOrder, context);

                    //Display Message for user to acknowledge
                    MessageBox.Show($"Order Deleted.");

                    //Reset Orderlistbox
                    OrderListBox.SelectedItem = null;
                    this.AddOrderIntoListBox();
                }
            }
        }
        
        /// <summary>
        /// Perfoemance Button Click event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PerformanceBtn_Click(object sender, RoutedEventArgs e)
        {
            PerformanceChart performanceChart = new PerformanceChart(_user);
            this.Close();
            performanceChart.Show();
        }

        /// <summary>
        /// Method to Get and Set portfolio data from DB into MarketExchange
        /// </summary>
        /// <param name="context"></param>
        private void SetAndBuildPortfolioData(DataContext context)
        {
            _user.GetMarketExchange().GetPortfolio().GetOrderList().Clear();
            _user.GetMarketExchange().GetPortfolio().GetOwnedStockList().Clear();
            _user.GetMarketExchange().GetPortfolio().GetTradeHistory().Clear();

            List <Stock> stockList = _user.GetMarketExchange().GetStocks();
            List<Order> orderList = new List<Order>();
            List<Trade> tradeList = new List<Trade>();
            List<OwnedStocks> ownedStockList = new List<OwnedStocks>();

            List<OrderDB> orderDBList = context.Order.ToList();
            List<TradeDB> tradeDBList = context.Trade.ToList();
            List<UserOwnedStockDB> ownedStockDBList = context.UserOwnedStock.ToList();

            //Add Order Object from DB into Portfolio (Part 1)
            foreach (OrderDB orderDB in orderDBList)
            {
                foreach (Stock stock in stockList)
                {
                    this.OrderSearchStockAndUser(orderDB, stock, orderList, _user.ID);
                }
            }

            //Add Trade Object from DB into Portfolio (Part 1)
            foreach (TradeDB tradeDB in tradeDBList)
            {
                foreach (Stock stock in stockList)
                {
                    this.TradeSearchStockAndUser(tradeDB, stock, tradeList, _user.ID);
                }
            }

            //Add OwnedStock Object from DB into Portfolio (Part 1)
            foreach (UserOwnedStockDB ownedStocksDB in ownedStockDBList)
            {
                foreach (Stock stock in stockList)
                {
                    this.OwnedStockSearchStockAndUser(ownedStocksDB, stock, ownedStockList, _user.ID);
                }
            }

            //Adding Order Objects into Portfolio
            this.AddOrderIntoMarketExchange(orderList, _user.GetMarketExchange());

            //Adding Trade Objects into Portfolio
            this.AddTradeIntoMarketExchange(tradeList, _user.GetMarketExchange());

            //Adding OwnedStock Objects into Portfolio
            //this.SortOwnedStockObject(ownedStockList, _user.GetMarketExchange());

            //Refactor OwnedStock DB
            //this.RefactorOwnedStockDB(ownedStockDBList, stockList, _user.ID, context);

            this.AddOwnedStocksIntoMarketExchange(ownedStockList, _user.GetMarketExchange());
        }

        /// <summary>
        /// Method to sort all OwnedStock based on Stock and OwnedType
        /// </summary>
        /// <param name="ownedStockList"></param>
        /// <param name="marketExchange"></param>
        private void SortOwnedStockObject(List<OwnedStocks> ownedStockList, MarketExchange marketExchange)
        {
            var groupedStocks = ownedStockList.GroupBy(stock => new { stock.Stock, stock.OwnedType });
            List<OwnedStocks> mergedStocks = new List<OwnedStocks>();

            foreach (var group in groupedStocks)
            {
                int totalQuantity = group.Sum(stock => stock.Quantity);
                //OwnedStocks mergedStock = new OwnedStocks(group.Key.Stock, totalQuantity, group.Key.OwnedType);
                //mergedStocks.Add(mergedStock);
            }

            this.AddOwnedStocksIntoMarketExchange(mergedStocks, marketExchange);
        }

        /// <summary>
        /// Method to refactor data inside Database
        /// </summary>
        /// <param name="ownedStockList"></param>
        /// <param name="stockList"></param>
        /// <param name="userID"></param>
        /// <param name="context"></param>
        private void RefactorOwnedStockDB(List<UserOwnedStockDB> ownedStockList, List<Stock> stockList, int userID, DataContext context)
        {
            foreach (Stock stock in stockList)
            {
                foreach (UserOwnedStockDB ownedStockDB in ownedStockList)
                {
                    context.UserOwnedStock.RemoveRange(context.UserOwnedStock.Where(ownedStockDB => ownedStockDB.UserID.Equals(userID) && ownedStockDB.StockID.Equals(stock.Id)));
                }
            }

            List<OwnedStocks> ownedStockListforDB = _user.GetMarketExchange().GetPortfolio().GetOwnedStockList();
            foreach (OwnedStocks stock in ownedStockListforDB)
            {
                UserOwnedStockDB userOwnedStockDB = new UserOwnedStockDB
                {
                    UserID = _user.ID,
                    OwnedType = stock.OwnedType,
                    StockID = stock.Stock.Id,
                    Quantity = stock.Quantity,
                    TotalValue = stock.TotalValue
                };
                context.Add(userOwnedStockDB);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Method to filter data from OwnedStockList with StockID and UserID
        /// </summary>
        /// <param name="ownedStocksDB"></param>
        /// <param name="stock"></param>
        /// <param name="ownedStockList"></param>
        /// <param name="userID"></param>
        private void OwnedStockSearchStockAndUser(UserOwnedStockDB ownedStocksDB, Stock stock, List<OwnedStocks> ownedStockList, int userID)
        {
            if (ownedStocksDB.UserID.Equals(userID) && stock.Id.Equals(ownedStocksDB.StockID))
            {
                this.BuildOwnedStockObject(ownedStocksDB.Id ,ownedStocksDB, stock, ownedStockList, ownedStocksDB.StockBoughtValue);
            }
        }

        /// <summary>
        /// Method to build OwnedStock object and add into ownedStockList
        /// </summary>
        /// <param name="ownedStocksDB"></param>
        /// <param name="stock"></param>
        /// <param name="ownedStockList"></param>
        private void BuildOwnedStockObject(int ID ,UserOwnedStockDB ownedStocksDB, Stock stock, List<OwnedStocks> ownedStockList, double StockBoughtValue)
        {
            OwnedStocks ownedStocks = new OwnedStocks(ID, stock, ownedStocksDB.Quantity, ownedStocksDB.OwnedType, StockBoughtValue);
            ownedStockList.Add(ownedStocks);
        }

        /// <summary>
        /// Method to Add Trade object Into MarketExchange
        /// </summary>
        /// <param name="tradeList"></param>
        /// <param name="marketExchange"></param>
        private void AddTradeIntoMarketExchange(List<Trade> tradeList, MarketExchange marketExchange)
        {
            foreach (Trade trade in tradeList)
            {
                marketExchange.AddTrade(trade);
            }
        }

        /// <summary>
        /// Method to Add OwnedStock object Into MarketExchange
        /// </summary>
        /// <param name="ownedStockList"></param>
        /// <param name="marketExchange"></param>
        private void AddOwnedStocksIntoMarketExchange(List<OwnedStocks> ownedStockList, MarketExchange marketExchange)
        {
            foreach (OwnedStocks ownedStock in ownedStockList)
            {
                marketExchange.AddOwnedStocks(ownedStock);
            }
        }

        /// <summary>
        /// Method to Add Order object Into MarketExchange
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="marketExchange"></param>
        private void AddOrderIntoMarketExchange(List<Order> orderList, MarketExchange marketExchange)
        {
            foreach (Order order in orderList)
            {
                marketExchange.AddOrder(order);
            }
        }

        /// <summary>
        /// Method to filter data from OrderList with StockID and UserID
        /// </summary>
        /// <param name="orderDB"></param>
        /// <param name="stock"></param>
        /// <param name="orderList"></param>
        /// <param name="userID"></param>
        private void OrderSearchStockAndUser(OrderDB orderDB, Stock stock, List<Order> orderList, int userID)
        {
            if (orderDB.UserID.Equals(userID) && stock.Id.Equals(orderDB.StockID))
            {
                this.DefineOrderObject(orderDB, stock, orderList);
            }
        }

        /// <summary>
        /// Method to filter data from TradeList with StockID and UserID
        /// </summary>
        /// <param name="tradeDB"></param>
        /// <param name="stock"></param>
        /// <param name="tradeList"></param>
        /// <param name="userID"></param>
        private void TradeSearchStockAndUser(TradeDB tradeDB, Stock stock, List<Trade> tradeList, int userID)
        {
            if (tradeDB.UserID.Equals(userID) && stock.Id.Equals(tradeDB.StockID))
            {
                this.DefineTradeObject(tradeDB, stock, tradeList);
            }
        }

        /// <summary>
        /// Method to define order object
        /// </summary>
        /// <param name="orderDB"></param>
        /// <param name="stock"></param>
        /// <param name="orderList"></param>
        private void DefineOrderObject(OrderDB orderDB, Stock stock, List<Order> orderList)
        {
            if (orderDB.OrderType.Equals("Buy"))
            {
                Stock orderStock = new Stock(stock.Id, stock.Name, stock.TickerSymbol, stock.Value);
                BuyOrder order = new BuyOrder(orderDB.OrderID, orderStock, orderDB.Quantity);
                orderList.Add(order);
            }
            else
            {
                Stock orderStock = new Stock(stock.Id, stock.Name, stock.TickerSymbol, stock.Value);
                ShortOrder order = new ShortOrder(orderDB.OrderID, orderStock, orderDB.Quantity);
                orderList.Add(order);
            }
        }

        /// <summary>
        /// Method to define trade object
        /// </summary>
        /// <param name="tradeDB"></param>
        /// <param name="stock"></param>
        /// <param name="tradeList"></param>
        private void DefineTradeObject(TradeDB tradeDB, Stock stock, List<Trade> tradeList)
        {
            if (tradeDB.TradeType.Equals("Buy"))
            {
                Stock tradeStock = new Stock(stock.Id, stock.Name, stock.TickerSymbol, stock.Value);
                BuyTrade trade = new BuyTrade(tradeStock, tradeDB.Quantity, tradeDB.BuyPrice, tradeDB.TotalBuyValue);
                tradeList.Add(trade);
            }
            else if (tradeDB.TradeType.Equals("Short"))
            {
                Stock tradeStock = new Stock(stock.Id, stock.Name, stock.TickerSymbol, stock.Value);
                ShortTrade trade = new ShortTrade(tradeStock, tradeDB.Quantity, tradeDB.BuyPrice, tradeDB.TotalBuyValue);
                tradeList.Add(trade);
            }
            else
            {
                Stock tradeStock = new Stock(stock.Id, stock.Name, stock.TickerSymbol, stock.Value);
                SellTrade trade = new SellTrade(tradeStock, tradeDB.Quantity, tradeDB.BuyPrice, tradeDB.TotalBuyValue);
                tradeList.Add(trade);
            }
        }

        /// <summary>
        /// Method to delete order from Database
        /// </summary>
        /// <param name="selectedOrder"></param>
        /// <param name="context"></param>
        private void DeleteOrderFromDB(Order selectedOrder, DataContext context)
        {
            OrderDB orderDB = new OrderDB
            {
                OrderID = selectedOrder.OrderID,
                OrderType = selectedOrder.OrderType,
                StockID = selectedOrder.StockID,
                UserID = _user.ID,
                Quantity = selectedOrder.Quantity
            };

            //Removing Order from portfolio and DB
            _user.GetMarketExchange().GetPortfolio().RemoveOrder(selectedOrder);
            context.Remove(orderDB);
            context.SaveChanges();
        }

        //Add Trade object into Portfolio and DB
        private void AddTradeIntoDatabase(TradeDB tradeDB)
        {
            using (DataContext context = new DataContext())
            {
                context.Trade.Add(tradeDB);
                context.SaveChanges();
            }
        }

        public void AddOwnedStockDetailsIntoListBox()
        {
            List<OwnedStocks> ownedStockList = _user.GetMarketExchange().GetPortfolio().GetOwnedStockList();
            OwnedStockColl = new ObservableCollection<OwnedStocks>(ownedStockList);
            OwnedStockListBox.ItemsSource = OwnedStockColl;
        }

        public void AddOrderIntoListBox()
        {
            List<Order> orderList = _user.GetMarketExchange().GetPortfolio().GetOrderList();
            OrderColl = new ObservableCollection<Order>(orderList);
            OrderListBox.ItemsSource = OrderColl;
        }

        public void AddTradeIntoListBox()
        {
            List<Trade> tradeList = _user.GetMarketExchange().GetPortfolio().GetTradeHistory();
            TradeColl = new ObservableCollection<Trade>(tradeList);
            TradeListBox.ItemsSource = TradeColl;
        }

        private void OwnedStockListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OwnedStockListBox.SelectedItem != null)
            {
                // Retrieve the selected item and cast it to your Stock class
                OwnedStocks selectedStock = OwnedStockListBox.SelectedItem as OwnedStocks;

                //Build View OwnedStockWindow and display it
                ViewOwnedStockWindow viewOwnedStockWindow = new ViewOwnedStockWindow(_user , selectedStock);
                this.Close();
                viewOwnedStockWindow.Show();
            }
        }

        /// <summary>
        /// Method that Executes Order Object to build a Trade object, Resetting ListBoxes and Adding Data Into Database.
        /// </summary>
        /// <param name="selectedOrder"></param>
        /// <param name="context"></param>
        private void AddTradeIntoDBChecker(Order selectedOrder, DataContext context)
        {
            if (selectedOrder.TotalValue > _user.GetMarketExchange().GetPortfolio().CurrentBalance)
            {
                MessageBox.Show($"You don't have enough funds to complete this trade.");
            }
            else
            {
                this.AddTradeIntoDB(selectedOrder,context);
                tab3.Focus();
            }
        }

        private void AddTradeIntoDB(Order selectedOrder, DataContext context)
        {
            Order order = selectedOrder as Order;
            _user.GetMarketExchange().GetPortfolio().AddTrade(order.MakeTrade());

            //Update User Balance
            _user.GetMarketExchange().GetPortfolio().CurrentBalance -= selectedOrder.TotalValue;
            _user.GetMarketExchange().GetPortfolio().AssetsValue += selectedOrder.TotalValue;
            _user.GetMarketExchange().GetPortfolio().CalculateTotalBalanceWithAssets();

            this.UpdateUserBalanceDatabase();

            TradeDB tradeDB = new TradeDB
            {
                TradeType = order.OrderType,
                StockID = order.StockID,
                UserID = _user.ID,
                Quantity = order.Quantity,
                BuyPrice = order.Value,
                TotalBuyValue = order.TotalValue,
                UserCurrentBalance = _user.GetMarketExchange().GetPortfolio().CurrentBalance,
                UserTotalBalance = _user.GetMarketExchange().GetPortfolio().TotalBalance
            };
            this.AddTradeIntoDatabase(tradeDB);
            this.DeleteOrderFromDB(selectedOrder, context);

            //Build and Add OwnedStock object into portfolio and Database
            this.AddOwnedStockIntoPortfolio(selectedOrder, context);

            //Reset Portfolio
            this.SetAndBuildPortfolioData(context);

            //Reset Orderlistbox
            this.ResetListBox();

            MessageBox.Show($"Trade success. Go to Trade history to view trade.");
        }

        private void AddOwnedStockIntoPortfolio(Order selectedOrder, DataContext context)
        {
            //Add OwnedStocks object into Portfolio
            OwnedStocks ownedStocks = new OwnedStocks(selectedOrder.GetOrderedStock(), selectedOrder.Quantity, selectedOrder.OrderType, selectedOrder.GetOrderedStock().Value);

            //Add OwnedStocks object into Database
            UserOwnedStockDB userOwnedStock = new UserOwnedStockDB
            {
                UserID = _user.ID,
                OwnedType = ownedStocks.OwnedType,
                StockID = ownedStocks.Stock.Id,
                Quantity = ownedStocks.Quantity,
                TotalValue = ownedStocks.TotalValue,
                StockBoughtValue = ownedStocks.Stock.Value
            };
            context.UserOwnedStock.Add(userOwnedStock);
            context.SaveChanges();
        }

        private void UpdateUserBalanceDatabase()
        {
            using (DataContext context = new DataContext())
            {
                UserDB user = context.Users.Find(_user.ID);
                if (user != null)
                {
                    user.CurrentBalance = _user.GetMarketExchange().GetPortfolio().CurrentBalance;
                    user.AssetsValue = _user.GetMarketExchange().GetPortfolio().AssetsValue;
                    context.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Method to reset and Update all ListBoxes
        /// </summary>
        private void ResetListBox()
        {
            OrderListBox.SelectedItem = null;
            TradeListBox.SelectedItem = null;
            OwnedStockListBox.SelectedItem = null;

            this.AddOrderIntoListBox();
            this.AddTradeIntoListBox();
            this.AddOwnedStockDetailsIntoListBox();
        }
    }
}
