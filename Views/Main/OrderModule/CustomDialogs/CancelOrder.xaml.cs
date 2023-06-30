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
        string sProduct;
        public CancelOrder(string order_id, string courier, string product)
        {
            InitializeComponent();
            tbOtherReason.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Height;
            tbOrderId.Text = order_id;
            this.sCourier = courier;
            this.sProduct = product;
            reasons();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            Cancel_api cancel_api = new Cancel_api();
            if(tbOtherReason.Visibility == Visibility.Visible)
            {
                if (cancel_api.api_cancel(tbOrderId.Text, tbOtherReason.Text, sCourier, sProduct))
                {
                    this.DialogResult = true;
                    Close();
                }
            }
            else
            {
                if (cancel_api.api_cancel(tbOrderId.Text, cbReason.Text, sCourier, sProduct))
                {
                    this.DialogResult = true;
                    Close();
                }
            }

        }
        public void reasons()
        {
            List<string> reasons = new List<string>();
            reasons.Add("Item is no longer needed.");
            reasons.Add("Found a better deal elsewhere.");
            reasons.Add("Ordered by mistake.");
            reasons.Add("Customer changed their mind.");
            reasons.Add("Incorrect item selected.");
            reasons.Add("Customer requested cancellation.");
            reasons.Add("Others.");

            cbReason.ItemsSource = reasons;
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

        private void cbReason_DropDownClosed(object sender, EventArgs e)
        {
            if (cbReason.Text == "Others.")
                tbOtherReason.Visibility = Visibility.Visible;
            else
                tbOtherReason.Visibility = Visibility.Collapsed;
        }
    }
}
