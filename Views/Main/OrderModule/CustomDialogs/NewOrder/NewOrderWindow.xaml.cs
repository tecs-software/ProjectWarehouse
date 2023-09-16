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
        public bool emptyFieldsChecker()
        {
            if (Util.IsAnyTextBoxEmpty(receiverInformationPage.tbFullName, receiverInformationPage.tbPhone, receiverInformationPage.tbAddress) || Util.IsAnyStringEmpty(receiverInformationPage.tbQuantity.Text, receiverInformationPage.tbGoodsValue.Text, receiverInformationPage.cbItem.Text, receiverInformationPage.tbWeight.Text))
            {
                return true;
            }
            else
            {
                return false;
                //validation for suspicious order
                //if (controller.SuspiciousValidation(receiverInformationPage.tbFirstName, receiverInformationPage.tbLastName, receiverInformationPage.tbPhone))
                //{
                //    CustomMessageBox("The data you will send has a matching record in TECS, and has value of RTS. Proceed with the booking?", true);
                //}
                //else
                //{
                //    mainFrame.Navigate(GetOrCreateBookingInformationPage());
                //}
            }
        }
        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (receiverInformationPage.rdbJandT.IsChecked == true)
            {
                if(emptyFieldsChecker())
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                else
                {
                    _receiver.FirstName = receiverInformationPage.tbFullName.Text;
                    _receiver.Phone = receiverInformationPage.tbPhone.Text;
                    _receiver.Province = receiverInformationPage.cbProvinceJnt.Text;
                    _receiver.City = receiverInformationPage.cbCityJnt.Text;
                    _receiver.Barangay = receiverInformationPage.cbBarangayJnt.Text;
                    _receiver.Address = receiverInformationPage.tbAddress.Text;

                    //booking frame
                    booking_info.item_name = receiverInformationPage.cbItem.Text;
                    booking_info.weight = receiverInformationPage.tbWeight.Text;
                    booking_info.goods_value = receiverInformationPage.tbGoodsValue.Text;
                    booking_info.bag_specification = receiverInformationPage.tbBagSpecification.Text;
                    booking_info.remarks = receiverInformationPage.tbRemarks.Text;
                    booking_info.quantity = receiverInformationPage.tbQuantity.Text;
                    booking_info.cod = decimal.Parse(receiverInformationPage.tbCod.Text);
                    btnNext.IsEnabled = false;
                    //calling the method for api ordering
                    if (queries.check_quantity(booking_info, _receiver))
                    {
                        if (await order_api.api_create(_receiver, booking_info, isSuspicious))
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
            else
            {
                if (emptyFieldsChecker())
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                else
                {
                    //receiver details
                    FLASHModel.receiver_name = receiverInformationPage.tbFullName.Text;
                    FLASHModel.receiver_phone = receiverInformationPage.tbPhone.Text;
                    FLASHModel.receiver_province = receiverInformationPage.cbProvinceFlash.Text;
                    FLASHModel.receiver_city = receiverInformationPage.cbCityFlash.Text;
                    FLASHModel.receiver_barangay = receiverInformationPage.cbBarangayFlash.Text;
                    FLASHModel.postal_code = receiverInformationPage.cbPostalCodeFlash.Text;
                    FLASHModel.receiver_address = receiverInformationPage.tbAddress.Text;

                    //parcel details
                    
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
