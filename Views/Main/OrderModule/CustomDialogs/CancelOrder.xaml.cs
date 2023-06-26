using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for CancelOrder.xaml
    /// </summary>
    public partial class CancelOrder : Window
    {
        string sCourier;
        public CancelOrder(string order_id, string courier)
        {
            InitializeComponent();
            tbOtherReason.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Height;
            tbOrderId.Text = order_id;
            this.sCourier = courier;
            reasons();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            Cancel_api cancel_api = new Cancel_api();
            if(cancel_api.api_cancel(tbOrderId.Text, cbReason.Text, sCourier))
            {
                this.DialogResult = true;
                Close();
            }

        }
        public void reasons()
        {
            List<String> reasons = new List<String>();
            reasons.Add("Item is no longer needed.");
            reasons.Add("Found a better deal elsewhere..");
            reasons.Add("Ordered by mistake.");
            reasons.Add("Customer changed their mind.");
            reasons.Add("Incorrect item selected.");
            reasons.Add("Customer requested cancellation.");

            for(int x = 0; x < 6;x++)
            {
                cbReason.Items.Add(reasons[x]);
            }
        }
        private void cbReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbReason.Text.ToLower().Contains("other"))
            {
                tbOtherReason.Visibility = Visibility.Visible;
                this.SizeToContent = SizeToContent.Height;
            }
            else
            {
                tbOtherReason.Visibility = Visibility.Collapsed;
                this.SizeToContent = SizeToContent.Height;
            }
        }
    }
}
