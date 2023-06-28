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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
                case "Courier Accounts":
                   // courierAccountFrame.Source = new Uri("../ProductView/ProductList.xaml", UriKind.Relative);
                    break;
                case "Sender Information":
                    //senderInfoFrame.Source = new Uri("../ProductView/ProductInformation.xaml", UriKind.Relative);
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

        }

        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
