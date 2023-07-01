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

namespace WarehouseManagement.Views.Main.DeliverModule
{
    /// <summary>
    /// Interaction logic for DeliveryTable.xaml
    /// </summary>
    public partial class DeliveryTable : UserControl
    {
        public DeliveryTable()
        {
            InitializeComponent();
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
            Show_order_inquiry.show_inquiry_data(tblProducts);
        }
        private void dialog(object sender)
        {
            refresh_table();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tblProducts_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Show_order_inquiry.show_inquiry_data(tblProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
