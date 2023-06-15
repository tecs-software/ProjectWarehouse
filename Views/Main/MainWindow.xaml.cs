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
using System.Windows.Shapes;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views.Login;
using WarehouseManagement.Views.Main.EmployeeModule;
using WarehouseManagement.Views.Main.InventoryModule;
using WarehouseManagement.Views.Main.OrderModule;

namespace WarehouseManagement.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDashboardModule_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnInventoryModule_Checked(object sender, RoutedEventArgs e)
        {
            if (!(PageContent.Content is InventoryView))
            {
                PageContent.Content = new InventoryView();
            }
        }
    

        private void btnOrderModule_Checked(object sender, RoutedEventArgs e)
        {
            if (!(PageContent.Content is OrderView))
            {
                PageContent.Content = new OrderView();
            }
        }

        private void btnEmployeeModule_Checked(object sender, RoutedEventArgs e)
        {
            if (!(PageContent.Content is EmployeeView))
            {
                PageContent.Content = new EmployeeView();
            }
        }

        private void btnSalesModule_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnComissionsModule_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnAccountDropDown_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item1 = new MenuItem() { Header = "Logout" };
            Util.ShowContextMenuForControl(sender as Control, item1);

            item1.Click += Logout_Click;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            //DBHelper.UpdateWorkHoursAndActiveUsers(CurrentUser.userID, DateTime.Now);
            //DBHelper.CloseConnection(DBHelper.GetConnection());
            var login = new LoginWindow();
            login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            login.Show();

            this.Close();
        }

    }
}
