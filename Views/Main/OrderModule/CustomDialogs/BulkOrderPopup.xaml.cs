using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WarehouseManagement.Controller;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for BulkOrderPopup.xaml
    /// </summary>
    public partial class BulkOrderPopup : Window
    {
        public BulkOrderPopup()
        {
            Csv_Controller.dataTableBulkOrders = null;
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
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

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!Csv_Controller.checkNullCells(dtBulkOrders) && !Csv_Controller.checkItemNameColumn(dtBulkOrders, cb))
            { }
            else
            {
                Csv_Controller.dataTableBulkOrders = Csv_Controller.PopulateToDataTable(dtBulkOrders);
                //Push to Create_API
                
            }
           
        }
    }
}
