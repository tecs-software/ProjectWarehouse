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
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using MenuItem = WarehouseManagement.Models.MenuItem;

namespace WarehouseManagement.Views.Main.InventoryModule
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : Page
    {
        public InventoryView()
        {
            InitializeComponent();
            showMenu();
        }

        private void tbSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void showMenu()
        {
            var menuCategory = new List<SubMenuItem>();
            menuCategory.Add(new SubMenuItem("Sale", 20));
            menuCategory.Add(new SubMenuItem("Highest Grossing", 20));
            menuCategory.Add(new SubMenuItem("Hot", 20));

            var category = new MenuItem("Category", menuCategory);

            var menuStatus = new List<SubMenuItem>();
            menuStatus.Add(new SubMenuItem("In-Stock", 30));
            menuStatus.Add(new SubMenuItem("Low-Stock", 40));
            menuStatus.Add(new SubMenuItem("Out of Stock", 50));

            var status = new MenuItem("Status", menuStatus);

            Menu.Children.Add(new InventoryMenu(category));
            Menu.Children.Add(new InventoryMenu(status));
        }

        private async void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct ap = new(null);

            if (ap.ShowDialog() == true)
            {
                await inventoryTable.RefreshDataGrid();
            }
        }

        private async void btnProfitAnalysis_Click(object sender, RoutedEventArgs e)
        {
            ProfitAnalysis pa = new(null)
            {
                mode = 2
            };

            if (pa.ShowDialog() == true)
            {
                await inventoryTable.RefreshDataGrid();
            }
        }
    }
}
