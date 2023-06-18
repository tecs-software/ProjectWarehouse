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
using WarehouseManagement.Controller;
using WWarehouseManagement.Database;
using WarehouseManagement.Helpers;
using System.Data;
using WarehouseManagement.Database;
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs;

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
            show_DT dt = new show_DT();
            dt.show_orders(dgtRespondentData);
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

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem() { Header = "Cancel Order" };
            System.Windows.Controls.MenuItem item2 = new System.Windows.Controls.MenuItem() { Header = "Check Status" };

            Util.ShowContextMenuForButton(sender as Button, item1, item2);

            item1.Click += Cancel_Order_Click;
            item2.Click += Check_Status_Click;
        }

        private void Check_Status_Click(object sender, RoutedEventArgs e)
        {
            if (dgtRespondentData.SelectedItems.Count > 0)
            {
                DataRowView? selectedRow = (DataRowView)dgtRespondentData.SelectedItems[0];

                if (selectedRow == null)
                    return;

                string? orderId = selectedRow["id"].ToString();


                CheckStatus cs = new CheckStatus();


                if (cs.ShowDialog() == true)
                {

                }
            }
        }


        private void Cancel_Order_Click(object sender, RoutedEventArgs e)
        {
            if (dgtRespondentData.SelectedItems.Count > 0)
            {
                DataRowView? selectedRow = (DataRowView)dgtRespondentData.SelectedItems[0];

                if (selectedRow == null)
                    return;

                string? status = selectedRow["status"].ToString();

                if(status.ToLower() == "pending")
                {
                    return;
                }

                CancelOrder ca = new CancelOrder();


                if (ca.ShowDialog() == true)
                {
                   
                }
            }
        }
    }
}
