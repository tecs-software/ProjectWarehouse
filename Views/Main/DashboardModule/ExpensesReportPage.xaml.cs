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
    /// Interaction logic for ExpensesReportPage.xaml
    /// </summary>
    public partial class ExpensesReportPage : Page
    {
        public ExpensesReportPage()
        {
            InitializeComponent();
            setChart();
        }

        private void setChart()
        {
            ChartValues<double> expensesData = new ChartValues<double>();
            Random random = new Random();
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                double expenses = random.Next(500000, 2000000) / 100.0; // Random expenses value between 500000 and 2000000, divided by 100 for decimal places
                expensesData.Add(expenses);
            }

            // Set the sample data to the expensesChart
            salesChart.Series.Clear();
            salesChart.Series.Add(new LineSeries
            {
                Title = "Total Expenses",
                Values = expensesData
            });
        }
    }
}
