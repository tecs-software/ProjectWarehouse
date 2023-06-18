using System;
using System.Collections.Generic;
using System.Data;
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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using MenuItem = System.Windows.Controls.MenuItem;

namespace WarehouseManagement.Views.Main.InventoryModule
{
    

    public partial class InventoryTable : UserControl
    {

        public InventoryTable()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateDataGrid();
        }

        private async void PopulateDataGrid()
        {
            await RefreshDataGrid();
        }

        public async Task RefreshDataGrid(Dictionary <string, string>? filters = null)
        {
            try
            {
                DBHelper dbHelper = new DBHelper();
                IEnumerable<string> columns = new List<string> { "product_id", "item_name", "nominated_price", "unit_quantity", "status", "reorder_point", "timestamp" }; // Specify your column names
                DataTable? dataTable = await dbHelper.GetTableFiltered("tbl_products", columns, filters);

                if (dataTable != null)
                {
                    tblProducts.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while refreshing the data grid: {ex.Message}");
            }
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Inventory"))
            {
                return;
            }

            MenuItem item3 = new MenuItem() { Header = "Mark Discontinued" };
            MenuItem item2 = new MenuItem() { Header = "Reduce Stock" };
            MenuItem item1 = new MenuItem() { Header = "Manage Product" };


            Util.ShowContextMenuForButton(sender as Button, item1, item2, item3);

            item3.Click += Discontinued_Click;
            item2.Click += Reduce_Stock_Click;
            item1.Click += Manage_Click;
        }

        private async void Reduce_Stock_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Inventory"))
            {
                return;
            }

            if (tblProducts.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)tblProducts.SelectedItems[0];

                Product selectedProduct = new Product
                {
                    ProductId = selectedRow["product_id"].ToString(),
                    ItemName = selectedRow["item_name"].ToString(),
                    UnitQuantity = Convert.ToInt32(selectedRow["unit_quantity"]),
                };

                ReduceStock rt = new ReduceStock(selectedProduct);


                if (rt.ShowDialog() == true)
                {
                    string status = rt.product.UnitQuantity < 0 ? Util.status_out_of_stock : (rt.product.UnitQuantity == 0 ? Util.status_out_of_stock : (rt.product.UnitQuantity <= 100 ? Util.status_low_stock : Util.status_in_stock));

                    DBHelper db = new DBHelper();

                    if (await db.UpdateData("tbl_products", new string[] { "unit_quantity", "status" }, new string[] { rt.product.UnitQuantity.ToString(), status }, "product_id", rt.product.ProductId))
                    {
                        await RefreshDataGrid();
                    }
                }
            }
        }

        private async void Manage_Click(object sender, RoutedEventArgs e)
        {
            if (tblProducts.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView) tblProducts.SelectedItems[0];

                Product selectedProduct = new Product
                {
                    ProductId = selectedRow["product_id"].ToString(),
                };

                AddProduct add = new(selectedProduct);


                if (add.ShowDialog() == true)
                {
                    await RefreshDataGrid();
                }
            }
        }

        private async void Discontinued_Click(object sender, RoutedEventArgs e)
        {
            if (tblProducts.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)tblProducts.SelectedItems[0];

                Product selectedProduct = new Product
                {
                    ProductId = selectedRow["product_id"].ToString(),
                };

                MessageBoxResult result = MessageBox.Show("Are you sure you want to discontinue this product?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DBHelper db = new DBHelper();

                    if (await db.UpdateData("tbl_products", new string[] { "status" }, new string[] { Util.status_discontinued }, "product_id", selectedProduct.ProductId))
                    {
                        await RefreshDataGrid();
                    }
                }
            }
        }
    }
}
