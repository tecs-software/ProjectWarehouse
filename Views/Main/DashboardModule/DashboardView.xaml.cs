using LiveCharts.Defaults;
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
using WarehouseManagement.Views.Main.OrderModule;
using WarehouseManagement.Database;

namespace WarehouseManagement.Views.Main.DashboardModule
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Page
    {
        private SalesReportPage? salesReportPage;
        private ExpensesReportPage? expensesReportPage;
        private SummaryPage? summaryPage;
        db_queries queries = new db_queries();

        public DashboardView()
        {
            InitializeComponent();
            summaryPage = new SummaryPage();
            cbSales.SelectedIndex = 0;
        }

        private void cbSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbSales.SelectedIndex)
            {
                case 0:
                    PageContent.Content = summaryPage;
                    break;
                case 1:
                    if (salesReportPage == null)
                        salesReportPage = new SalesReportPage();
                    PageContent.Content = salesReportPage;
                    break;
                case 2:
                    if (expensesReportPage == null)
                        expensesReportPage = new ExpensesReportPage();
                    PageContent.Content = expensesReportPage;
                    break;
            }
        }

        private void PageContent_Loaded(object sender, RoutedEventArgs e)
        {
            startDatePicker.DisplayDateEnd = DateTime.Now;
            endDatePicker.DisplayDateEnd = DateTime.Now;

            startDatePicker.SelectedDate = DateTime.Now.AddDays(-7);
            endDatePicker.DisplayDateStart = startDatePicker.SelectedDate;

            queries.sales_graph(startDatePicker,endDatePicker,summaryPage.salesChart);

        }

        private void startDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            endDatePicker.DisplayDateStart = startDatePicker.SelectedDate;
            queries.sales_graph(startDatePicker, endDatePicker, summaryPage.salesChart);
        }

        private void endDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            queries.sales_graph(startDatePicker, endDatePicker, summaryPage.salesChart);
        }
    }
}
