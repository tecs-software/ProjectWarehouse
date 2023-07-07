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
using WarehouseManagement.Controller;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Login;
using WarehouseManagement.Views.Main.DashboardModule;
using WarehouseManagement.Views.Main.DeliverModule;
using WarehouseManagement.Views.Main.EmployeeModule;
using WarehouseManagement.Views.Main.InventoryModule;
using WarehouseManagement.Views.Main.OrderModule;
using WarehouseManagement.Views.Main.SalesModule;
using WarehouseManagement.Views.Main.ShopModule;
using WarehouseManagement.Views.Main.SuspiciousModule;
using WarehouseManagement.Views.Main.SystemSettingModule;
using MenuItem = System.Windows.Controls.MenuItem;

namespace WarehouseManagement.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool initialPage = false;
        public MainWindow()
        {
            InitializeComponent();
            setUpUserAccess();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            tbAccountName.Text = CurrentUser.Instance.firstName;
            Trial_Controller.MessagePopup();

        }

        private void setUpUserAccess()
        {
            DBHelper db = new DBHelper();
            db.InsertOrUpdateWorkHoursAndActiveUsers(CurrentUser.Instance.userID, DateTime.Now);

            foreach (string moduleName in CurrentUser.Instance.ModuleAccessList)
            {
                switch (moduleName)
                {
                    case "View Dashboard":
                        btnDashboardModule.Visibility = Visibility.Visible;

                        if (!initialPage)
                        {
                            initialPage = true;
                            btnDashboardModule.IsChecked = true;
                            PageContent.Content = new DashboardView();
                        }

                        break;
                    case "View Order":
                        btnOrderModule.Visibility = Visibility.Visible;

                        if (!initialPage)
                        {
                            initialPage = true;
                            btnOrderModule.IsChecked = true;
                            PageContent.Content = new OrderView();
                        }

                        break;
                    case "View Inventory":
                        btnInventoryModule.Visibility = Visibility.Visible;

                        if (!initialPage)
                        {
                            initialPage = true;
                            btnInventoryModule.IsChecked = true;
                            PageContent.Content = new InventoryView();
                        }

                        break;
                    case "View Employee":

                        btnEmployeeModule.Visibility = Visibility.Visible;

                        if (!initialPage)
                        {
                            initialPage = true;
                            btnEmployeeModule.IsChecked = true;
                            PageContent.Content = new EmployeeView();
                        }

                        break;

                    // Add more cases for other module names if needed

                    default:
                        break;
                }
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
            if (!(PageContent.Content is DashboardView))
            {
                PageContent.Content = new DashboardView();
            }
        }

        private void btnInventoryModule_Checked(object sender, RoutedEventArgs e)
        {
            Trial_Controller.MessagePopup();

            if (!(PageContent.Content is InventoryView))
            {
                PageContent.Content = new InventoryView();
            }
        }
    

        private void btnOrderModule_Checked(object sender, RoutedEventArgs e)
        {
            Trial_Controller.MessagePopup();

            if (!(PageContent.Content is OrderView))
            {
                PageContent.Content = new OrderView();
            }
        }

        private void btnEmployeeModule_Checked(object sender, RoutedEventArgs e)
        {
            Trial_Controller.MessagePopup();

            if (!(PageContent.Content is EmployeeView))
            {
                PageContent.Content = new EmployeeView();
            }
        }

        private void btnSalesModule_Checked(object sender, RoutedEventArgs e)
        {
            Trial_Controller.MessagePopup();

            if (!(PageContent.Content is SalesView))
            {
                PageContent.Content = new SalesView();
            }
        }

        private void btnAccountDropDown_Click(object sender, RoutedEventArgs e)
        {
            showMenu(sender);
        }
        private void tbAccountName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

            if (CurrentUser.Instance.ModuleAccessList.Contains("Modify System Settings"))
            {
                MenuItem item3 = new MenuItem() { Header = "System Settings" };
                Util.ShowContextMenuForControl(sender as Control, item1, item2, item3);
                item3.Click += SystemSetting_Click;

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
        private void SystemSetting_Click(object sender, RoutedEventArgs e)
        {
            new SystemSettingPopup().ShowDialog();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DBHelper db = new DBHelper();
            db.UpdateWorkHoursAndActiveUsers(CurrentUser.Instance.userID, DateTime.Now);
            CurrentUser.Instance.Clear();
        }

        private void btn_minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnDeliver_Checked(object sender, RoutedEventArgs e)
        {
            Trial_Controller.MessagePopup();

            if (!(PageContent.Content is DeliveryView))
            {
                PageContent.Content = new DeliveryView();
            }
        }

        private void btn_Shops_Pages_Checked(object sender, RoutedEventArgs e)
        {
            if (!(PageContent.Content is ShopView))
            {
                PageContent.Content = new ShopView();
            }
        }

        private void btn_Suspicious_Order_Checked(object sender, RoutedEventArgs e)
        {
            if (!(PageContent.Content is SuspiciousView))
            {
                PageContent.Content = new SuspiciousView();
            }
        }
    }
}
