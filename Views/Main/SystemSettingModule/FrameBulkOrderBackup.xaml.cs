using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for FrameBulkOrderBackup.xaml
    /// </summary>
    public partial class FrameBulkOrderBackup : UserControl
    {
        BackgroundWorker pushOrders;
        
        void CustomMessageBox(String message, Boolean questionType)
        {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            txtMessageDialog.Text = message;
            if (questionType)
            {
                btnYes.Content = "Yes";
                btnNo.Visibility = Visibility.Visible;
            }
            else
            {
                btnYes.Content = "Okay";
                btnNo.Visibility = Visibility.Collapsed;
            }
            dialog.IsOpen = true;
        }

        public FrameBulkOrderBackup()
        {
            InitializeComponent();
            dtSuspiciousOrders.Visibility = Visibility.Collapsed;
            Csv_Controller.dataTableBulkOrders = null;
            Csv_Controller.SystemModel = new List<SystemSettingsModel>();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            dtBulkOrders.Items.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    //txtFileNameProduct.Text = openFileDialog.FileName;
                    Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                    int numberofitems = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName).Rows.Count;
                    pbBulkOrder.Maximum = numberofitems > 0 ? numberofitems : 100;
                    Csv_Controller.dataTablebulkOrder = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                    dtBulkOrders.ItemsSource = Csv_Controller.dataTablebulkOrder.DefaultView;

                    dtBulkOrders.Visibility = Visibility.Visible;
                    dtSuspiciousOrders.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //code dito
            try
            {
                Csv_Controller.dataTableBulkOrders = Csv_Controller.dataTablebulkOrder;
                foreach (DataRow dr in Csv_Controller.dataTableBulkOrders.Rows)
                {
                    SystemSettingsModel model = new SystemSettingsModel()
                    {
                        //receiver credentials
                        receiver_name = dr[4].ToString(),
                        receiver_phone = dr[5].ToString(),
                        receiver_address = dr[9].ToString(),

                        //other fields
                        remarks = dr[16].ToString(),
                        product_name = dr[23].ToString(),
                        quantity = int.Parse(dr[25].ToString()),

                        //etc
                        cod = decimal.Parse(dr[12].ToString()),
                        parcel_value = decimal.Parse(dr[30].ToString()),
                        parcel_name = dr[23].ToString(),
                        weight = decimal.Parse(dr[24].ToString()),

                        //id's
                        waybill = dr[3].ToString(),
                        order_id = dr[2].ToString(),
                        sender_name = dr[18].ToString()
                    };
                    Csv_Controller.SystemModel.Add(model);
                }

                btnConfirm.IsEnabled = false;
                pushOrders = new BackgroundWorker();
                pushOrders.WorkerReportsProgress = true;

                pushOrders.DoWork += WorkerPushOrders_DoWork;
                pushOrders.RunWorkerCompleted += WorkerPushCompleted_RunWorkerCompleted;

                pushOrders.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void WorkerPushOrders_DoWork(object sender, DoWorkEventArgs e)
        {
            bulk_inserts.insertBulkData(Csv_Controller.SystemModel, pbBulkOrder);
        }
        private void WorkerPushCompleted_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnConfirm.IsEnabled = true;
            MessageBox.Show("Data successfully inserted.");
        }
        private void dtBulkOrders_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dtSuspiciousOrders_AutoGeneratedColumns(object sender, EventArgs e)
        {

        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReConfirm_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
