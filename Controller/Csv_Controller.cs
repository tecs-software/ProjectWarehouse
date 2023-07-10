using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;
using System.Windows;
using System.Windows.Media;

namespace WarehouseManagement.Controller
{
    public class Csv_Controller
    {
        static sql_control sql = new sql_control();
        public static DataTable dataTableAddress { get; set; }
        public static Boolean ConfirmedToImport { get; set; }
        public static DataTable dataTableBulkOrders { get; set; }
        public static DataTable GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {

                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {

                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn dataColumn = new DataColumn(column);
                        dataColumn.AllowDBNull = true;
                        csvData.Columns.Add(dataColumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return csvData;
        }
        public static void ImportAddress(TextBlock txtCount, ProgressBar pbLoad)
        {
            sql.Query("EXEC SpAddress_Truncate");
            int totalImported = 0;
            foreach (DataRow dr in dataTableAddress.Rows)
            {
                sql.AddParam("@province", dr[0].ToString());
                sql.AddParam("@city", dr[1].ToString());
                sql.AddParam("@areaName", dr[2].ToString());
                sql.AddParam("@canDeliver", dr[3].ToString());

                sql.Query($"EXEC SpAddress_Import @province, @city, @areaName, @canDeliver ");
                if (sql.HasException(true)) return;

                totalImported++;
                //txtCount.Text = totalImported.ToString();
                txtCount.Dispatcher.Invoke(DispatcherPriority.Normal,
                new System.Action(() => { txtCount.Text = totalImported.ToString(); pbLoad.Value = totalImported; }));
            }
        }
        public static void insertItems(ComboBox cb)
        {
            cb.Items.Clear();
            if(CurrentUser.Instance.userID == 1)
            {
                sql.Query($"SELECT item_name FROM tbl_products");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        cb.Items.Add(dr[0].ToString());
                    }
                }
            }
            else
            {
                int? sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_users WHERE user_id = {CurrentUser.Instance.userID}"));
                sql.Query($"SELECT item_name FROM tbl_products WHERE sender_id = {sender_id}");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        cb.Items.Add(dr[0]);
                    }
                }
            }
        }
        public static bool checkNullCells(DataGrid dg) 
        {
            List<string> missingCells = new List<string>();

            foreach (DataRowView rowView in dg.Items)
            {
                DataRow row = rowView.Row;

                foreach (DataGridColumn column in dg.Columns)
                {
                    if (column is DataGridTextColumn textColumn)
                    {
                        string cellValue = row[textColumn.SortMemberPath]?.ToString();

                        if (string.IsNullOrEmpty(cellValue))
                        {
                            string cellInfo = $"Row {row.Table.Rows.IndexOf(row) + 1}, Column {column.Header}";
                            missingCells.Add(cellInfo);
                        }
                    }
                }
            }
            if (missingCells.Count > 0)
            {
                string message = "The following cells are missing or empty:\n" + string.Join("\n", missingCells);
                MessageBox.Show(message, "Missing Cells", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
                return true;
        }

        public static bool checkItemNameColumn(DataGrid dg, ComboBox cb)
        {
            List<string> missingItemNames = new List<string>();

            foreach (DataRowView rowView in dg.Items)
            {
                // Access the underlying DataRow
                DataRow row = rowView.Row;

                // Check if the row is empty
                if (row == null)
                {
                    missingItemNames.Add("Empty row found.");
                    continue;
                }

                // Loop through the columns in the row
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    // Access the value in the cell
                    var cellValue = row[i].ToString();

                    // Check if the cell value is empty
                    if (string.IsNullOrEmpty(cellValue))
                    {
                        int rowIndex = row.Table.Rows.IndexOf(row) + 1;
                        missingItemNames.Add($"Empty cell found in row {rowIndex}, column {i + 1}");
                        break; // Skip remaining cells in the row
                    }
                }
            }
            if(missingItemNames.Count > 0)
            {
                string message = "The following cells are missing or empty:\n" + string.Join("\n", missingItemNames);
                MessageBox.Show(message, "Missing item name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                return true;
            }
            
        }
        public static DataTable DataTable_Creation()
        {
            DataTable dt_BulkOrder = new DataTable();
            dt_BulkOrder.Clear();
            dt_BulkOrder.Columns.Add("Item Name");
            dt_BulkOrder.Columns.Add("Quantity");

            dt_BulkOrder.Columns.Add("Remarks");
            dt_BulkOrder.Columns.Add("Receiver Name");
            dt_BulkOrder.Columns.Add("Receiver Phone Number");
            dt_BulkOrder.Columns.Add("Receiver Address");
            dt_BulkOrder.Columns.Add("Receiver Province");
            dt_BulkOrder.Columns.Add("Receiver City");
            dt_BulkOrder.Columns.Add("Receiver Region");
            dt_BulkOrder.Columns.Add("Express Type");
            dt_BulkOrder.Columns.Add("Parcel Name");
            dt_BulkOrder.Columns.Add("Weight");
            dt_BulkOrder.Columns.Add("Total Parcel");
            dt_BulkOrder.Columns.Add("Parcel Value");
            dt_BulkOrder.Columns.Add("COD");

            return dt_BulkOrder;
        }
        public static DataTable PopulateToDataTable(DataGrid dataGrid)
        {
            DataTable dt_BulkOrder = DataTable_Creation();

            foreach (var item in dataGrid.Items)
            {
                var dataGridRow = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (dataGridRow != null)
                {
                    DataRow row = dt_BulkOrder.NewRow();
                    for (int columnIndex = 0; columnIndex < dataGrid.Columns.Count; columnIndex++)
                    {
                        var column = dataGrid.Columns[columnIndex];
                        if (column is DataGridTextColumn textColumn)
                        {

                            var cellContent = column.GetCellContent(dataGridRow);
                            if (cellContent is TextBlock textBlock)
                            { 
                                row[textColumn.Header.ToString()] = textBlock.Text;
                            }
                        }
                        else if (column is DataGridTemplateColumn templateColumn)
                        {
                            var cellContent = column.GetCellContent(dataGridRow);
                            if (cellContent is FrameworkElement frameworkElement)
                            {
                                var comboBox = FindVisualChild<ComboBox>(frameworkElement);
                                if (comboBox != null)
                                {
                                    row[templateColumn.Header.ToString()] = comboBox.Text;
                                }
                            }
                        }
                    }
                    dt_BulkOrder.Rows.Add(row);
                }
            }
            return dt_BulkOrder;
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T found)
                        return found;

                    var descendant = FindVisualChild<T>(child);
                    if (descendant != null)
                        return descendant;
                }
            }

            return null;
        }

    }
}
