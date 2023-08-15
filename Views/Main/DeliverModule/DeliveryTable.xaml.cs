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
using WarehouseManagement.Controller;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs;
using WarehouseManagement.Views.Main.DeliverModule;

namespace WarehouseManagement.Views.Main.DeliverModule
{
    /// <summary>
    /// Interaction logic for DeliveryTable.xaml
    /// </summary>
    public partial class DeliveryTable : UserControl
    {
        public static int offsetCount { get; set; } = 0;
        public DeliveryTable()
        {
            InitializeComponent();
        }
        private void SetColumnWidth()
        {
            
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth < 1920 || screenHeight < 1080)
            {
                waybillNo.Width = 150;
                productName.Width = 250;
                recieverName.Width = 250;
            }
            else
            {
                productName.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                recieverName.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                waybillNo.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            }
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem() { Header = "Delete Row" };

            Util.ShowContextMenuForButton(sender as Button, item1);

            item1.Click += Delete_Row_Click;
        }
        private async void Delete_Row_Click(object sender, RoutedEventArgs e)
        {
            if (tblProducts.SelectedItems.Count > 0)
            {
                object id = tblProducts.SelectedItem;
                string waybill = (tblProducts.SelectedCells[0].Column.GetCellContent(id) as TextBlock).Text;
                Show_order_inquiry.soft_delete(waybill);
            }
            refresh_table();
        }
        public void refresh_table()
        {

            Show_order_inquiry.show_inquiry_data(tblProducts,false);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetColumnWidth();
        }
        private void tblProducts_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DeliveryTable.offsetCount = 0;
                Show_order_inquiry.show_inquiry_data(tblProducts, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void tblProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            Show_order_inquiry.show_inquiry_data(tblProducts, false);
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Show_order_inquiry.show_inquiry_data(tblProducts, true);
        }
    }
}
