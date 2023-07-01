﻿using System;
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
using WarehouseManagement.Views.Main.OrderModule;

namespace WarehouseManagement.Views.Main.DeliverModule
{
    /// <summary>
    /// Interaction logic for DeliveryView.xaml
    /// </summary>
    public partial class DeliveryView : Page
    {
        public DeliveryView()
        {
            InitializeComponent();
        }
        private void tbSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
        private void dialog(object sender)
        {
            deliveryTable.refresh_table();
        }

        private void btnNewDelivery_Click(object sender, RoutedEventArgs e)
        {
            OrderInquiryPopup order_inquiry = new OrderInquiryPopup();
            order_inquiry.OnTableFilterRequested += dialog;
            deliveryTable.refresh_table();
        }
    }
}

