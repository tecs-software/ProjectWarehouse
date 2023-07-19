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
using System.Windows.Controls.Primitives;
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
        Dictionary<string, bulk_model> bulkDictionary;
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
        public BulkOrderPopup()
        {
            InitializeComponent();
            dtSuspiciousOrders.Visibility = Visibility.Collapsed;
            Csv_Controller.dataTableBulkOrders = null;
            Csv_Controller.model = new List<bulk_model>();
            bulkDictionary = new Dictionary<string, bulk_model>();
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

                    //dtBulkOrders.Columns[2].IsReadOnly = true;
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
            string idValue = "";
            cb = sender as ComboBox;
            Csv_Controller.insertItems(cb);
            DataGridCell dataGridCell = FindParent<DataGridCell>(cb);

            if (dataGridCell != null)
            {
                DataGridRow dataGridRow = FindParent<DataGridRow>(dataGridCell);
                if (dataGridRow != null)
                {
                    int columnIndex = 2; // Adjust this to the actual index of the ComboBox column
                    DataGridCellInfo cellInfo = new DataGridCellInfo(dataGridRow.Item, dtBulkOrders.Columns[columnIndex]);
                    DataGridCell cell = GetCell(dtBulkOrders, cellInfo);
                    if (cell != null && cell.Content is TextBlock textBlock)
                    {
                        idValue = textBlock.Text;
                        if (bulkDictionary.ContainsKey(idValue))
                            cb.Text = bulkDictionary[idValue].item_name;
                        else
                            cb.SelectedIndex = -1;
                    }
                }

            }

        }
        private void txtQuantity_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            DataGridCell dataGridCell = FindParent<DataGridCell>(tb);

            if (dataGridCell != null)
            {
                DataGridRow dataGridRow = FindParent<DataGridRow>(dataGridCell);

                if (dataGridRow != null)
                {
                    int columnIndex = 2;
                    DataGridCellInfo cellInfo = new DataGridCellInfo(dataGridRow.Item, dtBulkOrders.Columns[columnIndex]);
                    DataGridCell cell = GetCell(dtBulkOrders, cellInfo);
                    if (cell != null && cell.Content is TextBlock textBlock)
                    {
                        string idValue = textBlock.Text;
                        if (bulkDictionary.ContainsKey(idValue))
                            tb.Text = bulkDictionary[idValue].item_quantity;
                        else
                            tb.Text = string.Empty;
                    }
                }
            }
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
            ComboBox cmb = sender as ComboBox;
            object id = dtBulkOrders.SelectedItem;
            string selectedID = (dtBulkOrders.SelectedCells[2].Column.GetCellContent(id) as TextBlock).Text;

            DataGridRow row = FindVisualParent<DataGridRow>(cmb);

            if (row != null && row.IsSelected)
            {
                DataGridCell cell = GetCell(dtBulkOrders, row, 0);
                TextBox txtQuantity = FindVisualChild<TextBox>(cell);
                string quantity = txtQuantity.Text.ToString();

                bulk_model model = new bulk_model()
                {
                    ID = selectedID,
                    item_name = cmb.Text,
                    item_quantity = quantity
                };
                if (bulkDictionary.ContainsKey(selectedID))
                {
                    bulkDictionary[selectedID] = model;
                }
                else
                {
                    bulkDictionary.Add(selectedID, model);
                }
            }

        }

    

        private void dtSuspiciousOrders_AutoGeneratedColumns(object sender, EventArgs e)
        {
        }

        private void btnReConfirm_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox("Are you sure do you want to push these suspicious orders?",true);
        }

        private void txtQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            tb = sender as TextBox;

            object id = dtBulkOrders.SelectedItem;
            string selectedID = (dtBulkOrders.SelectedCells[2].Column.GetCellContent(id) as TextBlock).Text;

            DataGridRow row = FindVisualParent<DataGridRow>(tb);

            if (row != null && row.IsSelected)
            {
                DataGridCell cell = GetCell(dtBulkOrders, row, 0);
                ComboBox cmbItems = FindVisualChild<ComboBox>(cell);
                string selectedItem = cmbItems.SelectedItem?.ToString();


                bulk_model model = new bulk_model()
                {
                    ID = selectedID,
                    item_quantity = tb.Text,
                    item_name = selectedItem
                };
                if (bulkDictionary.ContainsKey(selectedID))
                {
                    bulkDictionary[selectedID] = model;
                }
                else
                {
                    bulkDictionary.Add(selectedID, model);
                }
            }
            
        }
        private DataGridCell GetCell(DataGrid dataGrid, DataGridRow row, int columnIndex)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }
        public DataGridCell GetCell(DataGrid dataGrid, DataGridCellInfo cellInfo)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    if (presenter != null)
                    {
                        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                        return cell;
                    }
                }
            }
            return null;
        }
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T result)
                {
                    return result;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }
        private T FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T result)
                {
                    return result;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        public T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }
    }
}
