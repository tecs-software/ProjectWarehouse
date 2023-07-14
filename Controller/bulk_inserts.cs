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
            int sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_products WHERE item_name = '{model.product_name}'"));

            sql.Query($"EXEC SPadd_orders '{order_id}', 'J&T', '{waybill}', {CurrentUser.Instance.userID}, '{model.product_name}'," +
                $"{model.quantity}, {model.total}, '{model.remarks}', 'PENDING', '{model.receiver_phone}', '{model.receiver_address}', {sender_id}");
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
            sql.Query($"SELECT * FROM tbl_bulk_order_temp");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {

            }
            else
            {
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
            }

        }

        public static void show_temp_table(DataGrid dgBulkOrder,DataGrid dgSuspicious, Button btn_confi, Button btn_reconfi)
        {
            sql.Query($"EXEC SpDisplay_SuspeciousBulk");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                MessageBox.Show("These are the suspicious datas");
                dgBulkOrder.Visibility = Visibility.Collapsed;
                dgSuspicious.Visibility = Visibility.Visible;

                btn_confi.Visibility = Visibility.Collapsed;
                btn_reconfi.Visibility = Visibility.Visible;

                // Set the DataTable as the ItemsSource for the DataGrid
                dgSuspicious.ItemsSource = sql.DBDT.DefaultView;
            }
            else
            {
                Csv_Controller.model.Clear();
                dgBulkOrder.ItemsSource = null;
            }
        }
        public static void show_new_temp_table(DataGrid dgBulkOrder, DataGrid dgSuspicious)
        {
            sql.Query($"EXEC SpDisplay_SuspeciousBulk");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                dgBulkOrder.Visibility = Visibility.Collapsed;
                dgSuspicious.Visibility = Visibility.Visible;

                // Set the DataTable as the ItemsSource for the DataGrid
                dgSuspicious.ItemsSource = sql.DBDT.DefaultView;
            }
            else
            {
                dgSuspicious.ItemsSource = null;
            }
        }
        public static void insertSuspiciousTable(string waybill)
        {
            sql.Query($"SELECT * FROM tbl_bulk_order_temp");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    int? role_id = int.Parse(sql.ReturnResult($"SELECT role_id FROM tbl_access_level WHERE user_id = {CurrentUser.Instance.userID}"));
                    string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name = '{dr[2].ToString()}'");
                    int? receiver_id = int.Parse(sql.ReturnResult($"SELECT TOP 1(receiver_id) FROM tbl_receiver WHERE receiver_name = '{dr[3].ToString()}' AND receiver_phone = '{dr[4].ToString()}'"));
                    decimal price = decimal.Parse(dr[12].ToString());
                    //sql.AddParam("@sender_id", GlobalModel.sender_id);
                    //sql.AddParam("@product_id", sql.ReturnResult($"SELECT product_id FROM tbl_orders WHERE item_name = '{dr[2].ToString()}'"));
                    //sql.AddParam("@receiver_id", sql.ReturnResult($"SELECT TOP 1(receiver_id) FROM tbl_receiver WHERE receiver_name = '{dr[3].ToString()}' AND receiver_phone = '{dr[4].ToString()}'"));
                    //sql.AddParam("@waybill", waybill);
                    //sql.AddParam("@courier", "J&T");
                    //sql.AddParam("@status", "PENDING");
                    //sql.AddParam("@booked_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //sql.AddParam("@price", decimal.Parse(dr[12].ToString()));

                    sql.Query($"INSERT INTO tbl_suspicious_order (user_id, role_id, sender_id, product_id, receiver_id, waybill, courier, status, booked_date, price) VALUES" +
                        $"({CurrentUser.Instance.userID}, {role_id}, {GlobalModel.sender_id}, '{product_id}', {receiver_id}, '{waybill}', 'J&T', 'PENDING', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {price})");
                    if (sql.HasException(true)) return;
                }
            }
        }
        public static void delete_suspicious_row(int id) => sql.Query($"DELETE FROM tbl_bulk_order_temp WHERE ID = {id}");
        public static void delete_temp_table() => sql.Query($"DELETE FROM tbl_bulk_order_temp");

        public static void load_bulk_model()
        {
            Csv_Controller.model.Clear();
            sql.Query($"SELECT * FROM tbl_bulk_order_temp");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
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
                        remarks = dr[14].ToString(),
                        product_name = dr[2].ToString(),
                        total = decimal.Parse(dr[13].ToString()),
                        quantity = int.Parse(dr[1].ToString()),

                        //etc
                        cod = decimal.Parse(dr[13].ToString()),
                        parcel_value = decimal.Parse(dr[12].ToString()),
                        parcel_name = dr[9].ToString(),
                        total_parcel = int.Parse(dr[11].ToString()),
                        weight = decimal.Parse(dr[10].ToString())


                    };
                    Csv_Controller.model.Add(model);
                }
            }
        }
    }
}
