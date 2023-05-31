using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace BrokerApp
{
    /// <summary>
    /// Interaction logic for PerformanceChart.xaml
    /// </summary>
    public partial class PerformanceChart : Window
    {
        private User _user;
        public PerformanceChart(User user)
        {
            InitializeComponent();
            _user = user;

            this.InitializeChart();

            //Clicke Event
            BackBtn.Click += (object sender, RoutedEventArgs e) => { BackBtn_Click(sender, e); };
        }

        public PlotModel MyPlotModel { get; set; }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            UserMainPage userMainPage = new UserMainPage(_user);
            this.Close();
            userMainPage.Show();
        }

        private void InitializeChart()
        {
            //Chart Name
            MyPlotModel = new PlotModel { Title = "Performance Chart" };

            // Create X axis with title
            var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Number Of Trades", MajorStep = 1 };
            MyPlotModel.Axes.Add(xAxis);

            // Create Y axis with title
            var yAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Account Balance", MajorStep = 10 };
            MyPlotModel.Axes.Add(yAxis);

            //Get Trade History Object
            List<double> balanceHistory = this.CalculateBalanceHistory();
            int counter = 0;

            if (balanceHistory.Count == 0) 
            {
                NoDataText.Text = "No Trade Data To Display";
            }
            else
            {
                //Insert Data into Chart
                var lineSeries = new LineSeries { Title = "Series 1" };
                foreach (double totalBalance in balanceHistory)
                {
                    ++counter;
                    lineSeries.Points.Add(new DataPoint(counter, totalBalance));
                }
                MyPlotModel.Series.Add(lineSeries);

                //Save DataContext (Chart)
                DataContext = this;
            }
        }

        private List<double> CalculateBalanceHistory()
        {
            List<double> BalanceHistory = new List<double>();
            using (DataContext context = new DataContext())
            {
                List<TradeDB> tradeDBList = context.Trade.Where(trade => trade.UserID.Equals(_user.ID)).ToList();
                foreach (TradeDB trade in tradeDBList)
                {
                    BalanceHistory.Add(trade.UserTotalBalance);
                }
            }
            return BalanceHistory;
        }
    }
}
