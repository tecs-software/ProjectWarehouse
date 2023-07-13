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
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;

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
            dtSuspiciousOrders.Visibility = Visibility.Collapsed;
            Csv_Controller.dataTableBulkOrders = null;
            Csv_Controller.model = new List<bulk_model>();
        }
        Create_api bulk_api = new Create_api();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            bulk_inserts.delete_temp_table();
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

                    dtBulkOrders.Visibility = Visibility.Visible;
                    dtSuspiciousOrders.Visibility = Visibility.Collapsed;
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
            System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem() { Header = "Delete Row" };

            Util.ShowContextMenuForButton(sender as Button, item1);

            item1.Click += Delete_Row_Click;
        }
        private async void Delete_Row_Click(object sender, RoutedEventArgs e)
        {
            object id = dtSuspiciousOrders.SelectedItem;
            string selectedID = (dtSuspiciousOrders.SelectedCells[1].Column.GetCellContent(id) as TextBlock).Text;
            bulk_inserts.delete_suspicious_row(int.Parse(selectedID));
            bulk_inserts.show_new_temp_table(dtBulkOrders, dtSuspiciousOrders);

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
        public static TextBox tb { get; set; }
        private void cmbSellerName_Loaded(object sender, RoutedEventArgs e)
        {
            cb = sender as ComboBox;
            Csv_Controller.insertItems(cb);
        }
        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!Csv_Controller.checkQuantityColumn(tb) && !Csv_Controller.checkItemNameColumn(dtBulkOrders, cb) && !Csv_Controller.checkNullCells(dtBulkOrders))
            {
                
            }
            else
            {
                try
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
                            total_parcel = int.Parse(dr[12].ToString()),
                            weight = decimal.Parse(dr[11].ToString())
                            

                        };
                        Csv_Controller.model.Add(model);
                    }
                    await bulk_api.create_bulk_api(Csv_Controller.model, btnConfirm, false);
                    if (btnConfirm.IsEnabled)
                    {
                        btnConfirm.IsEnabled = true;
                        MessageBox.Show("Orders has been Created");
                    }
                    bulk_inserts.show_temp_table(dtBulkOrders,dtSuspiciousOrders, btnConfirm, btnReConfirm);
                }
                catch (Exception ex)
                {

                }
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

        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            //Push to Create_API
            bulk_inserts.load_bulk_model();
            await bulk_api.create_bulk_api(Csv_Controller.model, btnReConfirm, true);
            if (btnReConfirm.IsEnabled)
            {
                btnReConfirm.IsEnabled = true;
                bulk_inserts.delete_temp_table();
                bulk_inserts.show_new_temp_table(dtBulkOrders,dtSuspiciousOrders);
                MessageBox.Show("Orders has been Created");
            }
        }
        private void cmbSellerName_DropDownClosed(object sender, EventArgs e)
        {
            
        }

        private void txtQuantity_Loaded(object sender, RoutedEventArgs e)
        {
            tb = sender as TextBox;
        }

        private void dtSuspiciousOrders_AutoGeneratedColumns(object sender, EventArgs e)
        {
        }

        private void btnReConfirm_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox("Are you sure do you want to push these suspicious orders?",true);
        }
    }
}
