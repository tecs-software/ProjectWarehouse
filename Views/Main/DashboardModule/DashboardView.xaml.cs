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
using WarehouseManagement.Models;
using WarehouseManagement.Controller;

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
        private VAPage? VaPage;

        db_queries queries = new db_queries();

        public DashboardView()
        {
            InitializeComponent();
            summaryPage = new SummaryPage();
            expensesReportPage = new ExpensesReportPage();
            VaPage = new VAPage();
            cbSales.SelectedIndex = 0;
            btn_D1.Focus();
            setDate();

            if (CurrentUser.Instance.RoleName != "admin")
            {
                PageContent.Content = VaPage;
                cmbContainer.Visibility = Visibility.Hidden;
            }
        }
        private void setDate()
        {
            DatePicker.DisplayDateStart = new DateTime(2023, 6, 1);
            DatePicker.DisplayDateEnd = DateTime.Now;
        }

        private void cbSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbSales.SelectedIndex)
            {
                case 0:
                    PageContent.Content = summaryPage;
                    break;
             
                case 1:
                    if (expensesReportPage == null)
                        expensesReportPage = new ExpensesReportPage();
                    PageContent.Content = expensesReportPage;
                    break;
                //case 1:
                //    if (salesReportPage == null)
                //        salesReportPage = new SalesReportPage();
                //    PageContent.Content = salesReportPage;
                //    break;
            }
        }

        private async void PageContent_Loaded(object sender, RoutedEventArgs e)
        {
            bulk_inserts.delete_temp_table();
            if (CurrentUser.Instance.RoleName == "admin")
            {
                dashboardDatas(1);
            }
        }

        private async void btn_D7_Click(object sender, RoutedEventArgs e)
        {
            dashboardDatas(7);
        }

        private async void btn_D1_Click(object sender, RoutedEventArgs e)
        {
            dashboardDatas(1);
        }

        private async void btn_D30_Click(object sender, RoutedEventArgs e)
        {
            dashboardDatas(30);
        }
        private async void dashboardDatas(int days)
        {
            // summary page
            await queries.sales_graph(days, summaryPage.salesChart);
            await ExpensesController.showTotalExpenses(summaryPage.lbl_expenses, days);
            await queries.load_dashboard_summary(summaryPage.lbl_total_order, summaryPage.lbl_gross, summaryPage.lbl_products_sold, summaryPage.lbl_Net_profit, days);
            expensesData(days);
        }
        private async void expensesData(int days)
        {
            await ExpensesController.showExpensesData(expensesReportPage.lbl_total_expenses, expensesReportPage.lbl_AdSpent, expensesReportPage.lbl_Utilities, expensesReportPage.lbl_Miscellaneous, days);
            ExpensesController.showExpensesGraphs(days, expensesReportPage.expensesChart);
        }

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (cbSales.SelectedIndex == 0)
            {
                queries.LoadDashboardByDate(summaryPage.lbl_total_order, summaryPage.lbl_gross, summaryPage.lbl_products_sold, summaryPage.lbl_Net_profit, summaryPage.salesChart, DatePicker);
                queries.LoadExpenseSummaryByDate(summaryPage.lbl_expenses, DatePicker);
            }
            else
            {
                queries.LoadExpenseDashBoardByDate(expensesReportPage.lbl_total_expenses, expensesReportPage.lbl_AdSpent, expensesReportPage.lbl_Utilities, expensesReportPage.lbl_Miscellaneous, expensesReportPage.expensesChart, DatePicker);
            }
            
        }
    }
}
