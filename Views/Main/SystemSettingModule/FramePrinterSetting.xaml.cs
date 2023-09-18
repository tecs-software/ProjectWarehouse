using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for FramePrinterSetting.xaml
    /// </summary>
    public partial class FramePrinterSetting : UserControl
    {
        void LoadAvailableDevice()
        {
            cmbFlashPrinter.Items.Clear();
            cmbJntPrinter.Items.Clear();

            foreach (String installedPrinters in PrinterSettings.InstalledPrinters)
            {
                cmbFlashPrinter.Items.Add(installedPrinters);
                cmbJntPrinter.Items.Add(installedPrinters);
            }
        }
        public FramePrinterSetting()
        {
            InitializeComponent();
            LoadAvailableDevice();
        }

        private async void btnSelect_Click_1(object sender, RoutedEventArgs e)
        {
            if(cmbJntPrinter.SelectedIndex == -1)
            {
                MessageBox.Show("Please select printer first.");
                return;
            }
            MessageBox.Show("Save Successfully.");
            await WaybillController.Save(cmbJntPrinter.Text, cmbFlashPrinter.Text);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await WaybillController.LoadDevice(cmbJntPrinter, cmbFlashPrinter);
        }
    }
}
