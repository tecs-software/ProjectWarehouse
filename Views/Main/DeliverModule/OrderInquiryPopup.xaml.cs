    using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using WarehouseManagement.Database;
using WarehouseManagement.Views.Main.DeliverModule;
using static WarehouseManagement.Controller.Create_api;

namespace WarehouseManagement.Views.Main.OrderModule
{
    /// <summary>
    /// Interaction logic for OrderInquiryPopup.xaml
    /// </summary>
    public partial class OrderInquiryPopup : Window
    {
        db_queries queries = new db_queries();
        Order_Inquiry_api order_Inquiry = new Order_Inquiry_api();
        public event EventHandler RefreshTable;
        string _session_id;
        public OrderInquiryPopup(string session_id)
        {
            InitializeComponent();
            txtBarcode.Focus();
            this._session_id = session_id;
        }
        public void showTable()
        {
            RefreshTable?.Invoke(this, EventArgs.Empty);
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private void clear_input_boxes()
        {
            txtBarcode.Text = "";
            txtReceiverName.Text = "";
            txtAddress.Text = "";
            txtBarangay.Text = "";
            txtCity.Text = "";
            txtContactNumber.Text = "";
            txtDateCreated.Text = "";
            txtProductName.Text = "";
            txtProvince.Text = "";
            txtQuantity.Text = "";
            txtRemarks.Text = "";
            txtWeight.Text = "";
            txtDateCreated.Text = "";
        }
        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            await order_Inquiry.insert_inquirt(txtBarcode.Text, txtReceiverName, txtContactNumber, txtAddress, txtProvince, txtCity, txtBarangay, txtDateCreated, txtRemarks, txtWeight, txtQuantity, txtProductName, txtDateCreated,_session_id);
            showTable();
            clear_input_boxes();
        }
        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Check if Enter key is pressed
            {
                if (txtBarcode.Text != "")
                {
                    await order_Inquiry.inquiry_api(txtBarcode.Text, txtReceiverName, txtContactNumber, txtAddress, txtProvince, txtCity, txtBarangay, txtDateCreated, txtRemarks, txtWeight, txtQuantity, txtProductName);
                    DisableTextBoxForOneSecond(txtBarcode);
                }
            }
        }
        private async void DisableTextBoxForOneSecond(TextBox textBox)
        {
            textBox.IsEnabled = false;

            await Task.Delay(1000); // Wait for 1 second

            textBox.IsEnabled = true;
            txtBarcode.Focus();
        }
    }
    
}
