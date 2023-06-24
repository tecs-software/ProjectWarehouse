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
        Customer _customer = new Customer();
        Receiver _receiver = new Receiver();
        Booking_info booking_info = new Booking_info();
        Create_api order_api = new Create_api();
        db_queries queries = new db_queries();

        public NewOrderWindow()
        {
            InitializeComponent();
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(GetOrCreateSenderInformationPage());
            this.SizeToContent = SizeToContent.Height;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame.Content is SenderInformation)
            {
                if(Util.IsAnyTextBoxEmpty(senderInformationPage.tbFirstName, senderInformationPage.tbLastName, senderInformationPage.tbPhone, senderInformationPage.tbAddress))
                {
                    //MessageBox.Show("Fill up required fields");
                    //return;
                }
                mainFrame.Navigate(GetOrCreateReceiverInformationPage());
                
            }
            else if (mainFrame.Content is ReceiverInformation)
            {
                
                mainFrame.Navigate(GetOrCreateBookingInformationPage());
            }
            else
            {
                //sender frame
                _customer.FirstName = senderInformationPage.tbFirstName.Text;
                _customer.MiddleName = senderInformationPage.tbMiddleName.Text;
                _customer.LastName = senderInformationPage.tbLastName.Text;
                _customer.Phone = senderInformationPage.tbPhone.Text;
                _customer.Province = senderInformationPage.cbProvince.Text;
                _customer.City = senderInformationPage.cbCity.Text;
                _customer.Barangay = senderInformationPage.cbBarangay.Text;
                _customer.Address = senderInformationPage.tbAddress.Text;

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
                booking_info.item_category = bookingInformationPage.tbCategory.Text;

                if(bookingInformationPage.radioJAndT.IsChecked == true)
                {
                    booking_info.courier = "JnT";
                }
                else if(bookingInformationPage.radioFlashExpress.IsChecked == true)
                {
                    booking_info.courier = "FlashExpress";
                }
                else if(bookingInformationPage.radioLBC.IsChecked == true)
                {
                    booking_info.courier = "LBC";
                }
                else
                {
                    booking_info.courier = "NinjaVan";
                }

                //calling the method for api ordering
                if(queries.deduct_inventory(booking_info, _customer, _receiver))
                {
                    if(order_api.api_create(_customer, _receiver, booking_info))
                    {
                        this.DialogResult = true;
                        this.Close();
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
        
    }
}
