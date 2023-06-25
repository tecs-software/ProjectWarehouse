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

namespace WarehouseManagement.Views.Main.OrderModule
{
    /// <summary>
    /// Interaction logic for ReturnSellerPopup.xaml
    /// </summary>
    public partial class ReturnSellerPopup : Window
    {
        public void reasons()
        {
            List<String> reasons = new List<String>();
            reasons.Add("Item is no longer needed.");
            reasons.Add("Found a better deal elsewhere..");
            reasons.Add("Ordered by mistake.");
            reasons.Add("Customer changed their mind.");
            reasons.Add("Incorrect item selected.");
            reasons.Add("Customer requested cancellation.");

            for (int x = 0; x < reasons.Count; x++)
            {
                cmbReason.Items.Add(reasons[x]);
            }
        }
        public ReturnSellerPopup()
        {
            InitializeComponent();
            Order_Controller.isConfirmedToReturn = false;
            reasons();
            txtBarcode.Focus();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (cmbReason.Text == "" || txtBarcode.Text == "")
                MessageBox.Show("Please complete all fields.");
            else if (!Order_Controller.isBarcodeExist(txtBarcode.Text))
                MessageBox.Show("The waybill is not exist");
            else if (Order_Controller.isBarcodeExist(txtBarcode.Text))
            {
                Order_Controller.UpdateStatus(Order_Controller.id);
                Order_Controller.isConfirmedToReturn = true; 
                Close();
            }
        }
    }
}
