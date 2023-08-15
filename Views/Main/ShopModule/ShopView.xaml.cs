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

namespace WarehouseManagement.Views.Main.ShopModule
{
    /// <summary>
    /// Interaction logic for ShopView.xaml
    /// </summary>
    public partial class ShopView : Page
    {
        ShopController shops = new ShopController();
        public static int offsetCount { get; set; } = 0;

        public ShopView()
        {
            InitializeComponent();
            insert_shops();
            SetColumnWidth();
            offsetCount = 0;
        }
        private void SetColumnWidth()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth < 1920 || screenHeight < 1080)
            {
                bookerName.Width = 250;
                waybillNo.Width = 200;
                productName.Width = 250;
                recieverName.Width = 250;
            }
            else
            {
                bookerName.Width = new DataGridLength(1.3, DataGridLengthUnitType.Star);
                waybillNo.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                productName.Width = new DataGridLength(1.3, DataGridLengthUnitType.Star);
                recieverName.Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            }
        }
        private void insert_shops()
        {
            cb_shops.SelectedIndex = 0;
            shops.populate_shops(cb_shops);
            shops.display_shop_data(dgt_shops, cb_shops,false);
        }
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cb_shops_DropDownClosed(object sender, EventArgs e)
        {
            shops.display_shop_data(dgt_shops, cb_shops, false);
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            shops.display_shop_data(dgt_shops, cb_shops, false);

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            shops.display_shop_data(dgt_shops, cb_shops, true);

        }
    }
}
