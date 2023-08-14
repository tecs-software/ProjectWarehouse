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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Controller;
using WarehouseManagement.Models;
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
            InitializeSession_id();
        }
        private void InitializeSession_id()
        {
            cmbSessions.Text = Order_Inquiry_api.setSession_id();
            GlobalModel.session_id = cmbSessions.Text;
            lblSessionID.Text = cmbSessions.Text;
            lbl_total_count.Text = Order_Inquiry_api.setParcelCount(cmbSessions.Text);
            Order_Inquiry_api.InsertSessions(cmbSessions);
        }
        private void tbSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
        private void Dialog(object sender, EventArgs e)
        {
            deliveryTable.refresh_table();
        }
        public string generateSessionID()
        {
            return Order_Inquiry_api.GenerateSession_id();
        }
        private void btnNewDelivery_Click(object sender, RoutedEventArgs e)
        {
            
            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Out For Pick Up"))
            {
                return;
            }

            OrderInquiryPopup orderInquiry = new OrderInquiryPopup(generateSessionID());
            orderInquiry.RefreshTable += Dialog;
            orderInquiry.Show();
            InitializeSession_id();
            deliveryTable.refresh_table();
        }

        private void cmbSellerName_DropDownClosed(object sender, EventArgs e)
        {
            GlobalModel.session_id = cmbSessions.Text;
            deliveryTable.refresh_table();
            lblSessionID.Text = cmbSessions.Text;
            lbl_total_count.Text = Order_Inquiry_api.setParcelCount(cmbSessions.Text);
        }
    }
}

