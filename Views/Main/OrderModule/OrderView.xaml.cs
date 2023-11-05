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
using WarehouseManagement.Views.Main.InventoryModule;

namespace WarehouseManagement.Views.Main.OrderModule
{
    public partial class OrderView : Page
    {
        static sql_control sql = new sql_control();
        public int pageCount { get; set; } = 1;
        public static int offsetCount { get; set; } = 0;

        public int TotalOrders = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_orders"));
        public int Completed = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_orders WHERE status = 'DELIVERED'"));
        public int Void = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_orders WHERE status = 'CANCELLED'"));
        public int inProgress = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_orders WHERE status != 'DELIVERED' AND status != 'CANCELLED'"));
        private void SetColumnWidth()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth < 1920 || screenHeight < 1080)
            {
                orderID.Width = 200;
                waybillNo.Width = 200;
                productName.Width = 250;
                recieverName.Width = 250;
            }
            else
            {
                orderID.Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
                waybillNo.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                productName.Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
                recieverName.Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            }
        }

        public OrderView()
        {
            InitializeComponent();
            showOrderMenu();
            SetColumnWidth();
            refreshTable();
        }
        private async void refreshTable()
        {
            show_DT dt = new show_DT();
            await dt.show_orders(dgtRespondentData, false);
            lblPageCount.Text = pageCount.ToString();
        }

        public async void showOrderMenu()
        {
            await updateMenu();

            var menuOrder = new List<SubMenuItem>();
            menuOrder.Add(new SubMenuItem("All", TotalOrders));
            menuOrder.Add(new SubMenuItem("Completed", Completed));
            menuOrder.Add(new SubMenuItem("Voided", Void));
            //menuOrder.Add(new SubMenuItem("Past Due", 3));
            menuOrder.Add(new SubMenuItem("In Progress", inProgress));
            var order = new MenuItem("Orders", menuOrder);

            Menu.Children.Clear();
            Menu.Children.Add(new OrderMenu(order));

        }
        public async Task updateMenu()
        {
            var menuOrder = new List<SubMenuItem>();
            menuOrder.Add(new SubMenuItem("All", TotalOrders));
            menuOrder.Add(new SubMenuItem("Completed", Completed));
            menuOrder.Add(new SubMenuItem("Voided", Void));
            //menuOrder.Add(new SubMenuItem("Past Due", 3));
            menuOrder.Add(new SubMenuItem("In Progress", inProgress));
            var order = new MenuItem("Orders", menuOrder);

            var inventoryMenu = Menu.Children.OfType<OrderMenu>().FirstOrDefault();
            if (inventoryMenu != null)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    inventoryMenu.DataContext = order;
                });
            }
        }
        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {

            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Order"))
            {
                return;
            }

            NewOrderWindow newOrderWindow = new NewOrderWindow();

            newOrderWindow.Owner = Window.GetWindow(this);


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
                string courier = (dgtRespondentData.SelectedCells[1].Column.GetCellContent(id) as TextBlock).Text;



                CheckStatus cs = new CheckStatus(waybill,courier);
                cs.Owner = Window.GetWindow(this);

                if (cs.ShowDialog() == true)
                {
                    refreshTable();
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

            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Order"))
            {
                return;
            }


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
                string courier = (dgtRespondentData.SelectedCells[1].Column.GetCellContent(id) as TextBlock).Text;
                string product = (dgtRespondentData.SelectedCells[3].Column.GetCellContent(id) as TextBlock).Text;

                CancelOrder ca = new CancelOrder(order_id, courier, product);

                ca.Owner = Window.GetWindow(this);

                if (ca.ShowDialog() == true)
                {
                    refreshTable();
                }

            }
        }

        private void btnReturntoSeller_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Order"))
            {
                return;
            }

            new ReturnSellerPopup().ShowDialog();
            if (Order_Controller.isConfirmedToReturn)
            {
                refreshTable();
            }
        }

        private void cb_shops_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cb_shops.SelectedIndex = 0;
        }

        private void btnBulkOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Order"))
            {
                return;
            }

            if (new BulkOrderPopup().ShowDialog() == true)
            {
                refreshTable();
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                show_DT.search_orders_data(tbSearch, rbtn_waybill, rbtn_customer, dgtRespondentData);
            }
        }

        private async void rbtn_show_all_Checked(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            refreshTable();
        }

        private async void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            show_DT dt = new show_DT();
            await dt.show_orders(dgtRespondentData, false);
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            show_DT dt = new show_DT();
            await dt.show_orders(dgtRespondentData, true);
        }

        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            show_DT.search_orders_data(tbSearch, rbtn_waybill, rbtn_customer, dgtRespondentData);
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
