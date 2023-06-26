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
        public CheckStatus(string waybill, string courier)
        {
            InitializeComponent();
            tbOrderId.Text = waybill;
            initialize_status();
            this.sCourier = courier;
        }
        public void initialize_status()
        {
            Track_api api_track = new Track_api();
            api_track.api_track(tbOrderId.Text, sCourier);
            api_track.update_status(tblStatus);
        }
    }
}
