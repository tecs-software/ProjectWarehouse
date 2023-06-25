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
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.LocalOrder;

namespace WarehouseManagement.Views.Main.OrderModule
{
    public partial class OrderView : Page
    {
       
        public OrderView()
        {
            InitializeComponent();
            showOrderMenu();

            //refreshTable();
            refreshTable();
            
        }

        private void refreshTable()
        {
            show_DT dt = new show_DT();
            dt.show_orders(dgtRespondentData);
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

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {

            NewOrderWindow newOrderWindow = new NewOrderWindow();

            if (newOrderWindow.ShowDialog() == true)
            {
                refreshTable();
            }

            //LocalOrderWindow localOrderWindow = new LocalOrderWindow();

            //if(localOrderWindow.ShowDialog() == true)
            //{
            //    refreshTable();
            //}

        }


        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem() { Header = "Cancel Order" };
            System.Windows.Controls.MenuItem item2 = new System.Windows.Controls.MenuItem() { Header = "Check Status" };

            Util.ShowContextMenuForButton(sender as Button, item1, item2);

            item1.Click += Cancel_Order_Click;
            item2.Click += Check_Status_Click;

            //System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem() { Header = "Void Order" };
            //System.Windows.Controls.MenuItem item2 = new System.Windows.Controls.MenuItem() { Header = "Complete Order" };

            //Util.ShowContextMenuForButton(sender as Button, item1, item2);


            //item1.Click += Cancel_Order_Click;
            //item2.Click += Check_Status_Click;
        }

        private async void Check_Status_Click(object sender, RoutedEventArgs e)
        {

            if (dgtRespondentData.SelectedItems.Count > 0)
            {
                var selectedOrder = dgtRespondentData.SelectedItems[0] as WarehouseManagement.Controller.Orders;

                if (selectedOrder == null)
                    return;

                string? orderId = selectedOrder.ID;



                object id = dgtRespondentData.SelectedItem;
                string waybill = (dgtRespondentData.SelectedCells[2].Column.GetCellContent(id) as TextBlock).Text;
                CheckStatus cs = new CheckStatus(waybill);


                if (cs.ShowDialog() == true)
                {

                }

            }

            //if (tblOrders.SelectedItems.Count > 0)
            //{
            //    DataRowView selectedRow = (DataRowView)tblOrders.SelectedItems[0];

            //    string orderID = selectedRow["order_id"].ToString();
            //    string status = selectedRow["status"].ToString();

            //    if(status != Util.status_in_progress)
            //    {

            //        return;
            //    }

            //    MessageBoxResult result = MessageBox.Show("Are you sure you want to complete this order?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //    if (result == MessageBoxResult.Yes)
            //    {
            //        DBHelper db = new DBHelper();

            //        if (await db.UpdateData("tbl_orders", new string[] { "status" }, new string[] { Util.status_completed }, "order_id", orderID))
            //        {
            //            refreshTable();
            //        }
            //    }
            //}
        }


        private async void Cancel_Order_Click(object sender, RoutedEventArgs e)
        {


            if (dgtRespondentData.SelectedItems.Count > 0)
            {
                var selectedOrder = dgtRespondentData.SelectedItems[0] as WarehouseManagement.Controller.Orders;

                if (selectedOrder == null)
                    return;

                string status = selectedOrder.status;

                if (status.ToLower() != "pending")
                {
                    return;
                }


                object id = dgtRespondentData.SelectedItem;
                string order_id = (dgtRespondentData.SelectedCells[0].Column.GetCellContent(id) as TextBlock).Text;

                CancelOrder ca = new CancelOrder(order_id);

                if (ca.ShowDialog() == true)
                {
                    // Code to handle the dialog result when it is true.
                }

            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnReturntoSeller_Click(object sender, RoutedEventArgs e)
        {
            new ReturnSellerPopup().ShowDialog();
            if (Order_Controller.isConfirmedToReturn)
            {
                refreshTable();
            }
        }



        //public async void refreshTable()
        //{
        //    string searchFilter =  tbSearch.Text;

        //    string? query;
        //    bool admin_privillages = false;

        //    if (CurrentUser.Instance.ModuleAccessList.Contains("Modify System Settings"))
        //    {
        //        admin_privillages = true;

        //        query = @"
        //            SELECT o.order_id, o.courier, u.username, r.receiver_name, p.item_name, o.quantity, o.total, o.status, o.remarks, o.created_at
        //            FROM tbl_orders o
        //            LEFT JOIN tbl_users u ON o.user_id = u.user_id
        //            LEFT JOIN tbl_receiver r ON o.receiver_id = r.receiver_id
        //            LEFT JOIN tbl_products p ON o.product_id = p.product_id
        //            ORDER BY o.created_at DESC";
        //    }
        //    else
        //    {
        //        query = $@"
        //            SELECT o.order_id, o.courier, r.receiver_name, p.item_name, o.quantity, o.total, o.status, o.remarks, o.created_at
        //            FROM tbl_orders o
        //            LEFT JOIN tbl_users u ON o.user_id = u.user_id
        //            LEFT JOIN tbl_receiver r ON o.receiver_id = r.receiver_id
        //            LEFT JOIN tbl_products p ON o.product_id = p.product_id
        //            WHERE o.user_id = {CurrentUser.Instance.userID}
        //            ORDER BY o.created_at DESC";
        //    }


        //    DataTable? dataTable = await DBHelper.GetTable(query);

        //    if (dataTable != null)
        //    {
        //        DataView dataView = new DataView(dataTable);

        //        foreach (DataRowView row in dataView)
        //        {
        //            row["receiver_name"] = Converter.CapitalizeWords(row["receiver_name"].ToString(), 2);
        //        }

        //        if (!admin_privillages)
        //        {
        //            usernameColumn.Visibility = Visibility.Collapsed;
        //        }

        //        tblOrders.ItemsSource = dataView;
        //    }
        //    else
        //    {
        //        MessageBox.Show("Failed to retrieve orders, database error.");
        //    }
        //}
    }
}
