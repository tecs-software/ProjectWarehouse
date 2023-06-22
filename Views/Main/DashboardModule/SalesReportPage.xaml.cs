using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarehouseManagement.Views.Main.DashboardModule
{
    /// <summary>
    /// Interaction logic for SalesReportPage.xaml
    /// </summary>
    public partial class SalesReportPage : Page
    {
        public SalesReportPage()
        {
            InitializeComponent();
            setChart();
        }

        private void setChart()
        {
            ChartValues<double> revenueData = new ChartValues<double>();
            Random random = new Random();
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                double revenue = random.Next(1000000, 5000000) / 100.0; // Random revenue value between 1000000 and 5000000, divided by 100 for decimal places
                revenueData.Add(revenue);
            }

            // Set the sample data to the salesChart
            salesChart.Series.Clear();
            salesChart.Series.Add(new LineSeries
            {
                Title = "Sales",
                Values = revenueData
            });
        }
    }
}
