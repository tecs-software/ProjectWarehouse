using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Data.SqlClient;
using WWarehouseManagement.Database;
using System.Windows.Controls;
using SixLabors.ImageSharp.Drawing.Processing;

namespace WarehouseManagement.Controller
{
    public class SuspiciousController
    {
        sql_control sql = new sql_control();
        public bool SuspiciousValidation(TextBox tb_name, TextBox tb_last, TextBox tb_phone)
        {
            string suspicious_name = tb_name.Text + " " + tb_last.Text;
            
            sql.Query($"SELECT receiver_id FROM tbl_orders WHERE status = 'CANCELLED' OR status = 'RTS' OR status = 'RETURNED TO SENDER'");

            if (sql.DBDT.Rows.Count > 0)
            {
                bool isMatchFound = false; // Variable to track if a match is found

                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    string? name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[0].ToString())}");
                    string? phone = sql.ReturnResult($"SELECT receiver_phone FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[0].ToString())}");

                    if (suspicious_name == name && tb_phone.Text == phone)
                    {
                        isMatchFound = true;
                        break; // Exit the loop since a match is found
                    }
                }
                return isMatchFound; // Return the result after the loop
            }
            return false; // Return false if no rows in tbl_orders
        }

        public void InsertSuspiciousData()
        {
            sql.Query($"SELECT TOP 1 * FROM tbl_orders");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    int? role_id = int.Parse(sql.ReturnResult($"SELECT role_id FROM tbl_access_level WHERE user_id = {int.Parse(dr[3].ToString())}"));

                    sql.AddParam("@user_id", int.Parse(dr[3].ToString()));
                    sql.AddParam("@role_id", role_id);
                    sql.AddParam("@sender_id", int.Parse(dr[4].ToString()));
                    sql.AddParam("@product_id", dr[6].ToString());
                    sql.AddParam("@receiver_id", dr[5].ToString());
                    sql.AddParam("@waybill", dr[2].ToString());
                    sql.AddParam("@courier", dr[1].ToString());
                    sql.AddParam("@status", dr[10].ToString());
                    sql.AddParam("@booked_date", DateTime.Parse(dr[11].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    sql.AddParam("@price", dr[8].ToString());


                    sql.Query($"INSERT INTO tbl_suspicious_order (user_id, role_id, sender_id, product_id, receiver_id, waybill, courier, status, booked_date, price) " +
                        $"VALUES (@user_id, @role_id, @sender_id, @product_id, @receiver_id, @waybill, @courier, @status, @booked_date, @price)");
                    if (sql.HasException(true)) return;
                }
            }
        }
        public void showSuspiciousData(DataGrid dg)
        {
            sql.Query($"SELECT * FROM tbl_suspicious_order ORDER BY suspicious_order_id DESC");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                List<suspicious> suspicious_Data = new List<suspicious>();
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    suspicious insert_data = new suspicious
                    {
                        booker_name = sql.ReturnResult($"SELECT first_name FROM tbl_users WHERE user_id = {int.Parse(dr[1].ToString())}"),
                        role = sql.ReturnResult($"SELECT role_name FROM tbl_roles WHERE role_id = {int.Parse(dr[2].ToString())}"),
                        shop = sql.ReturnResult($"SELECT sender_name FROM tbl_sender WHERE sender_id = {int.Parse(dr[3].ToString())}"),
                        product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '{dr[4].ToString()}'"),
                        receiver = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[5].ToString())}"),
                        waybill = dr[6].ToString(),
                        courier = dr[7].ToString(),
                        status = dr[8].ToString(),
                        booked_date = DateTime.Parse(dr[9].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        price = decimal.Parse(dr[10].ToString())
                    };
                    suspicious_Data.Add(insert_data);
                }
                dg.ItemsSource = suspicious_Data;
            }

        }
        public class suspicious
        {
            public string booker_name { get; set; }
            public string role { get; set; }
            public string shop { get; set; }
            public string product { get; set; }
            public string receiver { get; set; }
            public string waybill { get; set; }
            public string courier { get; set; }
            public string booked_date { get; set; }
            public decimal price { get; set; }
            public string status { get; set; }
        }
    }
}
