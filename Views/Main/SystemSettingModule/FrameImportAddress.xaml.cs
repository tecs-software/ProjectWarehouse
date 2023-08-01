using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for FrameImportAddress.xaml
    /// </summary>
    public partial class FrameImportAddress : UserControl
    {
        BackgroundWorker workerImportAddress;
        public FrameImportAddress()
        {
            InitializeComponent();
        }
        private void WorkerImportRegion_DoWork(object sender, DoWorkEventArgs e)
        {
            Csv_Controller.ImportAddress(lblImportedProducts, pbBarProduct);
        }
        private void WorkerImportRegion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Import address successfully", "Success");
            btnImportAddress.IsEnabled = true;
            Csv_Controller.ConfirmedToImport = true;
        }
        private void btnBrowseAddress_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFileNameProduct.Text = openFileDialog.FileName;
                Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                int numberofitems = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName).Rows.Count;
                pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
                lblTotalNumberOfItems.Text = numberofitems.ToString();
                Csv_Controller.dataTablebulkOrder = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
            }
        }

        private void btnImportAddress_Click(object sender, RoutedEventArgs e)
        {
            btnImportAddress.IsEnabled = false;
            workerImportAddress = new BackgroundWorker();
            workerImportAddress.WorkerReportsProgress = true;

            workerImportAddress.DoWork += WorkerImportRegion_DoWork;
            workerImportAddress.RunWorkerCompleted += WorkerImportRegion_RunWorkerCompleted;

            workerImportAddress.RunWorkerAsync();
        }
    }
}
