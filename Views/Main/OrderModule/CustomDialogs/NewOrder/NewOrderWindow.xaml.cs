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

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame.Content is ReceiverInformation)
            {
                if (Util.IsAnyTextBoxEmpty(receiverInformationPage.tbFirstName, receiverInformationPage.tbLastName, receiverInformationPage.tbPhone, receiverInformationPage.tbAddress))
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                mainFrame.Navigate(GetOrCreateBookingInformationPage());
            }
            else
            {
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
                if(queries.deduct_inventory(booking_info, _receiver))
                {
                    if(order_api.api_create(_receiver, booking_info, global_sender))
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
