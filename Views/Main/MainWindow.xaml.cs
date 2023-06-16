using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using WarehouseManagement.Models;
using WarehouseManagement.Views.Login;
using WarehouseManagement.Views.Main.EmployeeModule;
using WarehouseManagement.Views.Main.InventoryModule;
using WarehouseManagement.Views.Main.OrderModule;
using MenuItem = System.Windows.Controls.MenuItem;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setUpUserAccess(CurrentUser.Instance.accessLevel);
            tbAccountName.Text = CurrentUser.Instance.firstName;
        }

        private void setUpUserAccess(string accessLevel)
        {
            DBHelper db = new DBHelper();
            db.InsertOrUpdateWorkHoursAndActiveUsers(CurrentUser.Instance.userID, DateTime.Now);

            switch (accessLevel)
            {
                case "ADMIN":
                    break;
                case "Sales Agent":
                    
                    btnEmployeeModule.Visibility = Visibility.Collapsed;
                    btnComissionsModule.Visibility = Visibility.Collapsed;
                    btnSalesModule.Visibility = Visibility.Collapsed;
                    btnDashboardModule.Visibility = Visibility.Collapsed;
                    btnOrderModule_Checked(null, null);
                    break;
                case "Warehouse Manager":
                    btnOrderModule.Visibility = Visibility.Collapsed;
                    btnEmployeeModule.Visibility = Visibility.Collapsed;
                    btnComissionsModule.Visibility = Visibility.Collapsed;
                    btnSalesModule.Visibility = Visibility.Collapsed;
                    btnDashboardModule.Visibility = Visibility.Collapsed;
                    btnInventoryModule_Checked(null, null);
                    break;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && WindowState == WindowState.Normal)
            {
                DragMove();
            }
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
            showMenu(sender);
        }
        private void tbAccountName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //showMenu(sender);
        }

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //showMenu(sender);
        }

        private void showMenu(object? sender)
        {
            MenuItem item1 = new MenuItem() { Header = "Logout" };
            MenuItem item2 = new MenuItem() { Header = "Account Settings" };

            if(CurrentUser.Instance.accessLevel == "ADMIN")
            {
                MenuItem item3 = new MenuItem() { Header = "System Settings" };
                Util.ShowContextMenuForControl(sender as Control, item1, item2, item3);
            }
            else
            {
                Util.ShowContextMenuForControl(sender as Control, item1, item2);
            }
            
            

            item1.Click += Logout_Click;
        
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            DBHelper db = new DBHelper();
            db.UpdateWorkHoursAndActiveUsers(CurrentUser.Instance.userID, DateTime.Now);
            CurrentUser.Instance.Clear();
            var login = new LoginWindow();
            login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            login.Show();

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }
    }
}
