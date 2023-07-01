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

namespace WarehouseManagement.Views.Main.DashboardModule
{
    /// <summary>
    /// Interaction logic for VAPage.xaml
    /// </summary>
    public partial class VAPage : Page
    {
        public VAPage()
        {
            InitializeComponent();
        }
        show_VA_dashboard_data VA_Dashboard = new show_VA_dashboard_data();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VA_Dashboard.show_VA_data(lbl_commisions);
        }
    }
}
