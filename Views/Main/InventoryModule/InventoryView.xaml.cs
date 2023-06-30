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
using WarehouseManagement.Database;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using MenuItem = WarehouseManagement.Models.MenuItem;

namespace WarehouseManagement.Views.Main.InventoryModule
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : Page
    {
        private string status = null;
        private int discontinuedCount;
        private int lowStockCount;
        private int outOfStockCount;

        public InventoryView()
        {
            InitializeComponent();
            showMenu();
        }

        private async Task UpdateCounts()
        {
            DBHelper db = new DBHelper();
            (int discontinued, int lowStock, int outOfStock) counts = await db.GetProductsCount();
            discontinuedCount = counts.discontinued;
            lowStockCount = counts.lowStock;
            outOfStockCount = counts.outOfStock;

            await UpdateMenuItems(); // Update the menu items with the latest counts
        }

        private void tbSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            tableFilter(status);
        }

        private void Dialog_TableFilterRequested(object sender, string status)
        {
            tableFilter(status);
        }

        public async void tableFilter(string status)
        {
            this.status = status;
            Dictionary<string, string> filters = new Dictionary<string, string>();
            filters.Add("status", status);
            filters.Add("item_name", tbSearchProduct.Text);
            await inventoryTable.RefreshDataGrid(filters);
            await UpdateCounts();
        }

        public async void showMenu()
        {
            DBHelper db = new DBHelper();

            await UpdateCounts();


            var menuCategory = new List<SubMenuItem>();
            menuCategory.Add(new SubMenuItem("Sale", 20));
            menuCategory.Add(new SubMenuItem("Highest Grossing", 20));
            menuCategory.Add(new SubMenuItem("Hot", 20));

            var category = new MenuItem("Category", menuCategory);

            var menuStatus = new List<SubMenuItem>();
            menuStatus.Add(new SubMenuItem("Low-Stock", lowStockCount));
            menuStatus.Add(new SubMenuItem("Out of Stock", outOfStockCount));
            menuStatus.Add(new SubMenuItem("Discontinued", discontinuedCount));

            var status = new MenuItem("Status", menuStatus);

            Menu.Children.Clear();
            //Menu.Children.Add(new InventoryMenu(category));
            Menu.Children.Add(new InventoryMenu(status));
        }

        private async Task UpdateMenuItems()
        {
            var menuStatus = new List<SubMenuItem>();
            menuStatus.Add(new SubMenuItem("Low-Stock", lowStockCount));
            menuStatus.Add(new SubMenuItem("Out of Stock", outOfStockCount));
            menuStatus.Add(new SubMenuItem("Discontinued", discontinuedCount));

            var statusMenuItem = new MenuItem("Status", menuStatus);

            // Find the InventoryMenu control in the UI and update its DataContext
            var inventoryMenu = Menu.Children.OfType<InventoryMenu>().FirstOrDefault();
            if (inventoryMenu != null)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    inventoryMenu.DataContext = statusMenuItem;
                });
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if(!CurrentUser.Instance.ModuleAccessList.Contains("Modify Inventory"))
            {
                return;
            }

            AddProduct dialog = new AddProduct(null);
            dialog.TableFilterRequested += Dialog_TableFilterRequested;
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                tableFilter(null);
               
            }

            
        }

        private async void btnProfitAnalysis_Click(object sender, RoutedEventArgs e)
        {
            ProfitAnalysis pa = new(null)
            {
                mode = 2
            };

            pa.Owner = Window.GetWindow(this);

            if (pa.ShowDialog() == true)
            {
                await inventoryTable.RefreshDataGrid();
            }
        }
    }
}
