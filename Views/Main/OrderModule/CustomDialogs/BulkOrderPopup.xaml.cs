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

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for BulkOrderPopup.xaml
    /// </summary>
    public partial class BulkOrderPopup : Window
    {
        public BulkOrderPopup()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
