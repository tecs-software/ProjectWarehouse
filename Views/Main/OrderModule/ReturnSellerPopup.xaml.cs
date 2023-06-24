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
        string id;
        public ReturnSellerPopup()
        {
            InitializeComponent();
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
            //process of reading barcode.
            if(Order_Controller.isBarcodeExist(txtBarcode.Text))
            {
                id = Order_Controller.id;
                //trigger confirmation code here
                //after triggering confirmation -> if yes (Update the status to return to sender)
            }
        }
    }
}
