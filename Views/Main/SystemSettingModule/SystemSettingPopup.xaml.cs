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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for SystemSettingPopup.xaml
    /// </summary>
    public partial class SystemSettingPopup : Window
    {
        public SystemSettingPopup()
        {
            InitializeComponent();

        }
        db_queries queries = new db_queries();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            queries.province(cmbProvince);
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            //string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            //switch (tabItem)
            //{
            //    case "Product List":
            //        productListFrame.Source = new Uri("../ProductView/ProductList.xaml", UriKind.Relative);
            //        break;
            //    default:
            //        return;
            //}
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            switch (tabItem)
            {
                case "Sender Information":
                    //senderInfoFrame.Source = new Uri("../ProductView/ProductInformation.xaml", UriKind.Relative);
                    break;
                case "Courier Accounts":
                   // courierAccountFrame.Source = new Uri("../ProductView/ProductList.xaml", UriKind.Relative);
                    break;
                case "Import Address":
                    importAddressFrame.Source = new Uri("../SystemSettingModule/FrameImportAddress.xaml", UriKind.Relative);
                    break;
                default:
                    return;
            }
        }

        private void cmbCity_DropDownClosed(object sender, EventArgs e)
        {
            cmbBarangay.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.baranggay(cmbBarangay, cmbCity.Text);
        }

        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cmbCity.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.city(cmbCity, cmbProvince.Text);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(txtAddress, txtPagename, txtPhone) || Util.IsAnyComboBoxItemEmpty(cmbProvince) || Util.IsAnyComboBoxItemEmpty(cmbCity) || Util.IsAnyComboBoxItemEmpty(cmbBarangay))
            {
                MessageBox.Show("Please complete all required fields on sender information.");
                return;
            }
            else
            {
                if (queries.insert_sender(txtPagename, txtPhone, cmbProvince, cmbCity, cmbBarangay, txtAddress))
                {
                    MessageBox.Show("Shop/Page added");
                    txtAddress.Clear();
                    txtPagename.Clear();
                    txtPhone.Clear();
                    cmbProvince.Text = "";
                    cmbCity.Text = "";
                    cmbBarangay.Text = "";
                }
                else
                {
                    return;
                }
            }

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void txtAddress_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (HasSymbols(e.Text))
            {
                e.Handled = true; // Ignore the input
            }
        }
        private bool HasSymbols(string text)
        {
            foreach (char c in text)
            {
                // Check if the character is not alphanumeric, space, ",", ".", or "-"
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != ',' && c != '.' && c != '-')
                {
                    return true;
                }
            }
            return false;
        }
    }
}
