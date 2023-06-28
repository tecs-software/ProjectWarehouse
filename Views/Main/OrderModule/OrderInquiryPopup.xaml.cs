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

namespace WarehouseManagement.Views.Main.OrderModule
{
    /// <summary>
    /// Interaction logic for OrderInquiryPopup.xaml
    /// </summary>
    public partial class OrderInquiryPopup : Window
    {
        public OrderInquiryPopup()
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
        private void txtBarcode_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtBarcode.Text == "carl")
                txtProductName.Text = "Shampooday";
        }
    }
}
