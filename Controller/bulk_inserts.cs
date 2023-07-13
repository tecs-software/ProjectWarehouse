using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class bulk_inserts
    {
        static sql_control sql = new sql_control();
        

        public static void bulk_receiver(bulk_model model) 
        {
            sql.Query($"EXEC SPadd_receiver '{model.receiver_name}', '{model.receiver_phone}', '{model.receiver_address}'");
            if (sql.HasException(true)) return;
        }
        public static void bulk_orders(bulk_model model, string waybill, string order_id) 
        {
            sql.Query($"EXEC SPadd_orders '{order_id}', 'J&T', '{waybill}', {CurrentUser.Instance.userID}, '{model.product_name}'," +
                $"{model.quantity}, {model.total}, '{model.remarks}', 'PENDING', '{model.receiver_phone}', '{model.receiver_address}', {GlobalModel.sender_id}");
            if (sql.HasException(true)) return;
        }
        
        public static void bulk_incentives(bulk_model model, string order_id)
        {
            sql.Query($"EXEC SPadd_incentives {CurrentUser.Instance.userID}, '{order_id}', {model.quantity}," +
            $"{1}, '{model.product_name}'");
            if (sql.HasException(true)) return;
        }
        public static void bulk_update_quantity(bulk_model model)
        {
            sql.Query($"EXEC SPupdate_stocks {model.quantity}, '{model.product_name}'");
            if (sql.HasException(true)) return;
        }
        public static void bulk_update_stocks(bulk_model model)
        {
            int newStock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '{model.product_name}'"));
            string Status = newStock < 0 ? Util.status_out_of_stock : newStock == 0 ? Util.status_out_of_stock : newStock <= 100 ? Util.status_low_stock : Util.status_in_stock;
            sql.Query($"UPDATE tbl_products set status = '{Status}' WHERE item_name = '{model.product_name}'");
            if (sql.HasException(true)) return;
        }

        public static bool bulk_suspicious(bulk_model model)
        {
            bool isMatchFound = false; // Initialize the variable outside the condition

            sql.Query($"SELECT receiver_id FROM tbl_orders WHERE status = 'CANCELLED' OR status = 'RTS'");
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    string? name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[0].ToString())}");
                    string? phone = sql.ReturnResult($"SELECT receiver_phone FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[0].ToString())}");

                    if (model.receiver_name == name && model.receiver_phone == phone)
                    {
                        isMatchFound = true;
                        break; // Exit the loop since a match is found
                    }
                }
            }
            return isMatchFound; // Return the result after the condition
        }
        public static void bulk_temp_insert(bulk_model model)
        {
            MessageBox.Show("Weight"+model.weight.ToString());
            sql.AddParam("@quantity", model.quantity);
            sql.AddParam("@productName", model.product_name);
            sql.AddParam("@receiverName", model.receiver_name);
            sql.AddParam("@receiver_phone", model.receiver_phone);
            sql.AddParam("@receiver_address", model.receiver_address);
            sql.AddParam("@receiver_province", model.receiver_province);
            sql.AddParam("@receiver_city", model.receiver_city);
            sql.AddParam("@receiver_area", model.receiver_area);
            sql.AddParam("@parcel_name", model.parcel_name);

            sql.AddParam("@weight", model.weight);
            sql.AddParam("@total_parcel", model.total_parcel);
            sql.AddParam("@parcel_value", model.parcel_value);
            sql.AddParam("@cod", model.cod);
            sql.AddParam("@remarks", model.remarks);

            sql.Query($"EXEC SPadd_temptable @quantity, @productName, @receiverName, @receiver_phone, @receiver_address, @receiver_province, @receiver_city, @receiver_area, @parcel_name, " +
            $" @weight, @total_parcel, @parcel_value, @cod, @remarks ");
            if (sql.HasException(true)) return;
            MessageBox.Show("RTS");
        }

        public static void show_temp_table(DataGrid dg)
        {
            sql.Query($"SELECT * FROM tbl_bulk_order_temp");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                MessageBox.Show("These are the suspicious datas");
                // Clear the existing columns
                dg.Columns.Clear();

                // Create columns based on the database column names
                foreach (DataColumn column in sql.DBDT.Columns)
                {
                    DataGridTextColumn dataGridColumn = new DataGridTextColumn();
                    dataGridColumn.Header = column.ColumnName;
                    dataGridColumn.Binding = new Binding(column.ColumnName);
                    dg.Columns.Add(dataGridColumn);
                }

                // Clear the existing items
                dg.ItemsSource = null;

                // Set the DataTable as the ItemsSource for the DataGrid
                dg.ItemsSource = sql.DBDT.DefaultView;
            }
        }
    }
}
