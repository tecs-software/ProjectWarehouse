﻿    using System;
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
        public event EventHandler<string> refresh_table;
        public OrderInquiryPopup()
        {
            InitializeComponent();
            txtBarcode.Focus();
        }
        public void OnTableFilterRequested()
        {
            refresh_table?.Invoke(this, null);
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            await order_Inquiry.insert_inquirt(txtBarcode.Text, txtReceiverName, txtContactNumber, txtAddress, txtProvince, txtCity, txtBarangay, txtDateCreated, txtRemarks, txtWeight, txtQuantity, txtProductName, txtDateCreated);
            
        }
        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Check if Enter key is pressed
            {
                if (txtBarcode.Text != "")
                {
                    await order_Inquiry.inquiry_api(txtBarcode.Text, txtReceiverName, txtContactNumber, txtAddress, txtProvince, txtCity, txtBarangay, txtDateCreated, txtRemarks, txtWeight, txtQuantity, txtProductName);
                }
            }
        }
    }
    
}
