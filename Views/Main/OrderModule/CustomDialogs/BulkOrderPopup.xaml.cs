using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using WarehouseManagement.Controller;
using WarehouseManagement.Models;

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
            Csv_Controller.dataTableBulkOrders = null;
            Csv_Controller.model = new List<bulk_model>();
        }
        Create_api bulk_api = new Create_api();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }
        

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    //txtFileNameProduct.Text = openFileDialog.FileName;
                    Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                    int numberofitems = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName).Rows.Count;
                    //pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
                    //lblTotalNumberOfItems.Text = numberofitems.ToString();
                    Csv_Controller.dataTableAddress = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                    dtBulkOrders.ItemsSource = Csv_Controller.dataTableAddress.DefaultView;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dtBulkOrders_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(decimal))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = $"#,##0.#0";

                }
            }
        }
        public static ComboBox cb { get; set; }
        private void cmbSellerName_Loaded(object sender, RoutedEventArgs e)
        {
            cb = sender as ComboBox;
            Csv_Controller.insertItems(cb);
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!Csv_Controller.checkNullCells(dtBulkOrders) && !Csv_Controller.checkItemNameColumn(dtBulkOrders, cb))
            {
                
            }
            else
            {
                Csv_Controller.dataTableBulkOrders = Csv_Controller.PopulateToDataTable(dtBulkOrders);
                //Push to Create_API
                foreach (DataRow dr in Csv_Controller.dataTableBulkOrders.Rows)
                {
                    bulk_model model = new bulk_model()
                    {
                        //receiver payload
                        receiver_name = dr[3].ToString(),
                        receiver_address = dr[5].ToString(),
                        receiver_phone = dr[4].ToString(),
                        receiver_province = dr[6].ToString(),
                        receiver_city = dr[7].ToString(),
                        receiver_area = dr[8].ToString(),

                        //other fields
                        remarks = dr[2].ToString(),
                        product_name = dr[0].ToString(),
                        total = decimal.Parse(dr[13].ToString()),
                        quantity = int.Parse(dr[1].ToString()),

                        //etc
                        cod = decimal.Parse(dr[14].ToString()),
                        parcel_value = decimal.Parse(dr[13].ToString()),
                        parcel_name = dr[10].ToString(),
                        total_parcel = int.Parse(dr[12].ToString())

                    };
                    Csv_Controller.model.Add(model);
                }
                await bulk_api.create_bulk_api(Csv_Controller.model, false, btnConfirm);
                if (btnConfirm.IsEnabled)
                {
                    btnConfirm.IsEnabled = true;
                    MessageBox.Show("Orders has been Created");
                }
                bulk_inserts.show_temp_table(dtBulkOrders);
            }
        }
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
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
