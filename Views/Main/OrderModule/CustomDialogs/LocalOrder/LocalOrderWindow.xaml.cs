using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.LocalOrder
{
    /// <summary>
    /// Interaction logic for LocalOrderWindow.xaml
    /// </summary>
    public partial class LocalOrderWindow : Window
    {

        private LocalBookingInformation? localBookingInformation;
        private LocalReceiverInformation? localReceiverInformation;
        Receiver receiver = new Receiver();

        public LocalOrderWindow()
        {
            InitializeComponent();
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(GetOrCreateReceiverInformation());
            this.SizeToContent = SizeToContent.Height;
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame.Content is LocalReceiverInformation)
            {
                if(Util.IsAnyStringEmpty(
                    localReceiverInformation.tbFirstName.Text, 
                    localReceiverInformation.tbLastName.Text,
                    localReceiverInformation.tbPhone.Text,
                    localReceiverInformation.tbAddress.Text,
                    localReceiverInformation.cbProvince.Text,
                    localReceiverInformation.cbCity.Text,
                    localReceiverInformation.cbBarangay.Text
                    ))
                {
                    MessageBox.Show("Please complete all required fields");
                    return;
                }

                receiver.FirstName = localReceiverInformation.tbFirstName.Text.Trim();
                receiver.MiddleName = localReceiverInformation.tbMiddleName.Text.Trim();
                receiver.LastName = localReceiverInformation.tbLastName.Text.Trim();
                receiver.Phone = localReceiverInformation.tbPhone.Text.Trim();
                receiver.Province = localReceiverInformation.cbProvince.Text;
                receiver.City = localReceiverInformation.cbCity.Text;
                receiver.Barangay = localReceiverInformation.cbBarangay.Text;
                receiver.Address = localReceiverInformation.tbAddress.Text.Trim();

                mainFrame.Navigate(GetOrCreateBookingInformation());
            }
            else
            {
                if (Util.IsAnyStringEmpty(
                    localBookingInformation.cbItem.Text,
                    localBookingInformation.tbQuantity.Text
                    ))
                {
                    MessageBox.Show("Please complete all required fields");
                    return;
                }

                string? courier = null;

                if (localBookingInformation.radioJAndT.IsChecked.GetValueOrDefault(false))
                    courier = "J & T";
                else if (localBookingInformation.radioFlashExpress.IsChecked.GetValueOrDefault(false))
                    courier = "Flash Express";
                else if (localBookingInformation.radioLBC.IsChecked.GetValueOrDefault(false))
                    courier = "LBC";
                else if (localBookingInformation.radioNinjavan.IsChecked.GetValueOrDefault(false))
                    courier = "Ninjavan";

                if(courier == null)
                {
                    MessageBox.Show("Please select a courier.");
                    return;
                }

                DBHelper db = new DBHelper();
                string productId = ((Product)localBookingInformation.cbItem.SelectedItem).ProductId;
                string remakrs = localBookingInformation.tbRemarks.Text.Trim();
                decimal price =  Converter.StringToDecimal(((Product)localBookingInformation.cbItem.SelectedItem).NominatedPrice.ToString());
                int quantity = Converter.StringToInteger(localBookingInformation.tbQuantity.Text);
                int stock = Converter.StringToInteger(await db.GetValue("tbl_products", "unit_quantity", "product_id", productId));
                decimal total = quantity * price;
               

                if (quantity > stock)
                {
                    MessageBox.Show("Not Enough Stock");
                    return;
                }

                int newStock = stock - quantity;

                if (!string.IsNullOrEmpty(receiver.MiddleName.Trim()))
                {
                    receiver.MiddleName = receiver.MiddleName.Trim() + " ";
                }

                string[] receiverDataValues = new string[]
                    {
                        receiver.FirstName + " " + receiver.MiddleName + receiver.LastName,
                        receiver.Phone,
                        receiver.Address
                    };

                string Status = newStock < 0 ? Util.status_out_of_stock : newStock == 0 ? Util.status_out_of_stock : newStock <= 100 ? Util.status_low_stock : Util.status_in_stock;

                string[] orderValues = new string[]
                {
                        "DA" + new Random().Next(10000, 99999).ToString(),
                        courier,
                        CurrentUser.Instance.userID.ToString(),
                        productId,
                        quantity.ToString(),
                        total.ToString(),
                        remakrs,
                        Status,
                };

                if (await db.InsertOrder(receiverDataValues, orderValues))
                {
                    MessageBox.Show("Order added successfully");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Order failed");
                    return;
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

        private LocalReceiverInformation GetOrCreateReceiverInformation()
        {
            if (localReceiverInformation == null)
            {
                localReceiverInformation = new LocalReceiverInformation();
            }

            return localReceiverInformation;
        }

        private LocalBookingInformation GetOrCreateBookingInformation()
        {
            if (localBookingInformation == null)
            {
                localBookingInformation = new LocalBookingInformation();

            }

            return localBookingInformation;
        }
    }  
}
