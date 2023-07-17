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
            SetColumnWidth();
        }
    }
}
