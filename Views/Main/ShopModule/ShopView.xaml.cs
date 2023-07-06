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
        public ShopView()
        {
            InitializeComponent();
            insert_shops();
        }
        private void insert_shops()
        {
            cb_shops.SelectedIndex = 0;
            shops.populate_shops(cb_shops);
            shops.display_shop_data(dgt_shops, cb_shops);
        }
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cb_shops_DropDownClosed(object sender, EventArgs e)
        {
            shops.display_shop_data(dgt_shops, cb_shops);
        }
    }
}
