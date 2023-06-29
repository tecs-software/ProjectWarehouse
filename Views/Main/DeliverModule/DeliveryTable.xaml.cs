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
        Show_order_inquiry show_parcel_data = new Show_order_inquiry();
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }
        public void refresh_table()
        {
            show_parcel_data.show_inquiry_data(tblProducts);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tblProducts_Loaded(object sender, RoutedEventArgs e)
        {
            show_parcel_data.show_inquiry_data(tblProducts);
        }
    }
}
