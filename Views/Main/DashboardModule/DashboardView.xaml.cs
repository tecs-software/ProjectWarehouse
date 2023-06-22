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
    }
}
