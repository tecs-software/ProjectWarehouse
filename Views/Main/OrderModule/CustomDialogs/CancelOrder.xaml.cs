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
    /// Interaction logic for CancelOrder.xaml
    /// </summary>
    public partial class CancelOrder : Window
    {
        public CancelOrder()
        {
            InitializeComponent();
            tbOtherReason.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.Height;
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
