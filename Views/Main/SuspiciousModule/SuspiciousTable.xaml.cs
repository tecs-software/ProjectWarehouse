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

namespace WarehouseManagement.Views.Main.SuspiciousModule
{
    /// <summary>
    /// Interaction logic for SuspiciousTable.xaml
    /// </summary>
    public partial class SuspiciousTable : UserControl
    {
        public SuspiciousTable()
        {
            InitializeComponent();
            
        }
        SuspiciousController suspicious_controller = new SuspiciousController();
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }
        public void ShowSuspiciousData()
        {
            suspicious_controller.showSuspiciousData(tblProducts);
        }

        private void tblProducts_Loaded(object sender, RoutedEventArgs e)
        {
            ShowSuspiciousData();
        }
    }
}
