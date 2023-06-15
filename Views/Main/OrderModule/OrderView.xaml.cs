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
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;
using MenuItem = WarehouseManagement.Models.MenuItem;

namespace WarehouseManagement.Views.Main.OrderModule
{
    /// <summary>
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderView : Page
    {
        public OrderView()
        {
            InitializeComponent();
            showOrderMenu();
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrderWindow newOrderWindow = new NewOrderWindow();

            if (newOrderWindow.ShowDialog() == true)
            {

            }
        }

        public void showOrderMenu()
        {
            var menuOrder = new List<SubMenuItem>();
            menuOrder.Add(new SubMenuItem("All", 20));
            menuOrder.Add(new SubMenuItem("Completed", 10));
            menuOrder.Add(new SubMenuItem("Voided", 5));
            menuOrder.Add(new SubMenuItem("Past Due", 3));
            menuOrder.Add(new SubMenuItem("In Progress", 2));
            var order = new MenuItem("Orders", menuOrder);

            Menu.Children.Add(new OrderMenu(order));
        }
    }
}
