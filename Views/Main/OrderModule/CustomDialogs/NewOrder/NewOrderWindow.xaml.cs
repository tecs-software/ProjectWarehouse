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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder
{
    /// <summary>
    /// Interaction logic for NewOrderWindow.xaml
    /// </summary>
    /// 
    
    public partial class NewOrderWindow : Window
    {
        private SenderInformation senderInformationPage;
        private ReceiverInformation receiverInformationPage;
        private BookingInformation bookingInformationPage;

        //models
        GlobalModel global_sender = new GlobalModel();
        Receiver _receiver = new Receiver();
        Booking_info booking_info = new Booking_info();
        Create_api order_api = new Create_api();
        db_queries queries = new db_queries();

        void CustomMessageBox(String message, Boolean questionType)
        {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            txtMessageDialog.Text = message;
            if (questionType)
            {
                btnYes.Content = "Yes";
                btnNo.Visibility = Visibility.Visible;
            }
            else
            {
                btnYes.Content = "Okay";
                btnNo.Visibility = Visibility.Collapsed;
            }
            dialog.IsOpen = true;
        }

        public NewOrderWindow()
        {
            InitializeComponent();
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(GetOrCreateReceiverInformationPage());
            this.SizeToContent = SizeToContent.Height;
        }
        SuspiciousController controller = new SuspiciousController();
        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame.Content is ReceiverInformation)
            {
                if (Util.IsAnyTextBoxEmpty(receiverInformationPage.tbFirstName, receiverInformationPage.tbLastName, receiverInformationPage.tbPhone, receiverInformationPage.tbAddress))
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                else
                {
                    //validation for suspicious order
                    if (controller.SuspiciousValidation(receiverInformationPage.tbFirstName, receiverInformationPage.tbPhone))
                    {
                        CustomMessageBox("The data you will send has a matching record in TECS, and has value of RTS. Proceed with the booking?", true);
                    }
                    else
                    {
                        mainFrame.Navigate(GetOrCreateBookingInformationPage());
                    }
                }
               
            }
            else
            {
                if (Util.IsAnyStringEmpty(bookingInformationPage.tbQuantity.Text, bookingInformationPage.tbGoodsValue.Text, bookingInformationPage.cbItem.Text, bookingInformationPage.tbWeight.Text))
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                queries.get_sender(global_sender);

                //receiver frame
                _receiver.FirstName = receiverInformationPage.tbFirstName.Text;
                _receiver.MiddleName = receiverInformationPage.tbMiddleName.Text;
                _receiver.LastName = receiverInformationPage.tbLastName.Text;
                _receiver.Phone = receiverInformationPage.tbPhone.Text;
                _receiver.Province = receiverInformationPage.cbProvince.Text;
                _receiver.City = receiverInformationPage.cbCity.Text;
                _receiver.Barangay = receiverInformationPage.cbBarangay.Text;
                _receiver.Address = receiverInformationPage.tbAddress.Text;

                //booking frame
                booking_info.item_name = bookingInformationPage.cbItem.Text;
                booking_info.weight = bookingInformationPage.tbWeight.Text;
                booking_info.goods_value = bookingInformationPage.tbGoodsValue.Text;
                booking_info.bag_specification = bookingInformationPage.tbBagSpecification.Text;
                booking_info.remarks = bookingInformationPage.tbRemarks.Text;
                booking_info.quantity = bookingInformationPage.tbQuantity.Text;

                btnNext.IsEnabled = false;
                //calling the method for api ordering
                if (queries.check_quantity(booking_info, _receiver))
                {
                    if (await order_api.api_create(_receiver, booking_info, global_sender, isSuspicious))
                    {
                        btnNext.IsEnabled = true;
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        btnNext.IsEnabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Not enought stocks for the desired quantity.");
                }
            }
        }

        private void btnBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mainFrame.CanGoBack)
            {
                mainFrame.GoBack();
            }
        }

        private SenderInformation GetOrCreateSenderInformationPage()
        {
            if (senderInformationPage == null)
            {
                senderInformationPage = new SenderInformation();
            }

            return senderInformationPage;
        }

        private ReceiverInformation GetOrCreateReceiverInformationPage()
        {
            if (receiverInformationPage == null)
            {
                receiverInformationPage = new ReceiverInformation();
            }

            return receiverInformationPage;
        }

        private BookingInformation GetOrCreateBookingInformationPage()
        {
            if (bookingInformationPage == null)
            {
                bookingInformationPage = new BookingInformation();
            }
            
            return bookingInformationPage;
        }
        public bool isSuspicious { get; set; } = false;
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if(txtMessageDialog.Text == "The data you will send has a matching record in TECS, and has value of RTS. Proceed with the booking?")
            {
                isSuspicious = true;
            }
            mainFrame.Navigate(GetOrCreateBookingInformationPage());

        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
