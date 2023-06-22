using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Database
{
    public class db_queries
    {
        sql_control sql = new sql_control();

        public void insert_sender(Customer _customer)
        {
            string name = _customer.FirstName + " " + _customer.LastName;
            sql.Query($"INSERT INTO tbl_sender (sender_name, sender_phone, sender_address) VALUES ('" + name + "', '" + _customer.Phone + "', '" + _customer.Address + "')");
            if (sql.HasException(true)) return;
        }
        public void insert_receiver(Receiver _receiver)
        {
            string name = _receiver.FirstName + " " + _receiver.LastName;
            sql.Query($"INSERT INTO tbl_receiver (receiver_name, receiver_phone, receiver_address) VALUES ('" + name + "', '" + _receiver.Phone + "', '" + _receiver.Address + "')");
            if (sql.HasException(true)) return;
        }
        public void Insert_Orders(string order_id, string waybill, Booking_info book_info)
        {
            string user_id = sql.ReturnResult($"SELECT user_id FROM tbl_users");
            string sender_id = sql.ReturnResult($"SELECT sender_id FROM tbl_sender ORDER BY sender_id DESC");
            string receiver_id = sql.ReturnResult($"SELECT receiver_id FROM tbl_receiver ORDER BY receiver_id DESC");
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name ='" + book_info.item_name + "'");
            decimal total = decimal.Parse(book_info.quantity) * decimal.Parse(book_info.goods_value);

            sql.Query($"INSERT INTO tbl_orders (order_id, waybill_number, user_id, sender_id, receiver_id, product_id, quantity, total, remarks, status, created_at, updated_at, courier) VALUES" +
                $"('" + order_id + "', '" + waybill + "', '" + user_id + "', '" + sender_id + "', '" + receiver_id + "', '" + product_id + "', '" + book_info.quantity + "', '" + total + "', '" + book_info.remarks + "'," +
                "'Pending', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','','"+book_info.courier+"')");
            if (sql.HasException(true)) return;
        }
    }
}
