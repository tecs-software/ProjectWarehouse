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
using System.Windows.Shapes;
using WarehouseManagement.Controller;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for CheckStatus.xaml
    /// </summary>
    public partial class CheckStatus : Window
    {
        string sCourier;
        update_order_status order_status = new update_order_status();
        Track_api api_track = new Track_api();

        public CheckStatus(string waybill, string courier)
        {
            InitializeComponent();
            tbOrderId.Text = waybill;
            sCourier = courier;
            initialize_status();
        }
        public async void initialize_status()
        {
            await api_track.api_track(tbOrderId.Text, sCourier);
            await api_track.update_status(tblStatus);
        }

        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            await order_status.get_order_status(tbOrderId.Text);
            this.DialogResult = true;
        }
    }
}
