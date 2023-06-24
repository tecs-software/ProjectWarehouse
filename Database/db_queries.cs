using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WarehouseManagement.Controller;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Database
{
    public class db_queries
    {
        sql_control sql = new sql_control();
        GlobalModel gModel = new GlobalModel();

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
            string sender_id = sql.ReturnResult($"SELECT sender_id FROM tbl_sender ORDER BY sender_id DESC");
            string receiver_id = sql.ReturnResult($"SELECT receiver_id FROM tbl_receiver ORDER BY receiver_id DESC");
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name ='" + book_info.item_name + "'");
            decimal total = decimal.Parse(book_info.quantity) * decimal.Parse(book_info.goods_value);

            sql.Query($"INSERT INTO tbl_orders (order_id, waybill_number, user_id, sender_id, receiver_id, product_id, quantity, total, remarks, status, created_at, updated_at, courier) VALUES" +
                $"('" + order_id + "', '" + waybill + "', '" + CurrentUser.Instance.userID + "', '" + sender_id + "', '" + receiver_id + "', '" + product_id + "', '" + book_info.quantity + "', '" + total + "', '" + book_info.remarks + "'," +
                "'Pending', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','','"+book_info.courier+"')");
            if (sql.HasException(true)) return;
        }

        public void insert_Incentives(Booking_info book_Info)
        {
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name = '"+book_Info.item_name+"'");
            sql.Query($"SELECT employee_commission FROM tbl_selling_expenses WHERE product_id = '"+product_id+"'");
            if(sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                   decimal commisions = decimal.Parse(dr[0].ToString());

                    decimal total_commi = commisions * decimal.Parse(book_Info.quantity);

                    sql.Query($"INSERT INTO tbl_incentives (user_id, incentive_for, quantity, total_incentive, is_valid) " +
                        $"VALUES ('"+CurrentUser.Instance.userID+"', '"+product_id+"', '"+int.Parse(book_Info.quantity)+"', '"+total_commi+"', 1)");
                    if (sql.HasException(true)) return;

                }
            }
        }
        public void get_userID(TextBox txt_username)
        {
            gModel.user_id = sql.ReturnResult($"SELECT user_id FROM tbl_users WHERE username = '"+txt_username.Text+"'");
        }
        public void province(ComboBox cb)
        {
            sql.Query($"SELECT distinct province FROM tbl_address_delivery WHERE province != '' ORDER BY province ASC");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                List<string> provinces = new List<string>();
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    provinces.Add(dr[0].ToString());
                }

                cb.ItemsSource = provinces;
            }
        }
        public void city(ComboBox cb, string province)
        {
            sql.Query($"SELECT distinct city FROM tbl_address_delivery WHERE province = '"+province+ "' AND CanDeliver = '1' ORDER BY city ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                List<string> cities = new List<string>();
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    cities.Add(dr[0].ToString());
                }

                cb.ItemsSource = cities;
            }
        }
        public void baranggay(ComboBox cb, string city)
        {
            sql.Query($"SELECT distinct AreaName FROM tbl_address_delivery WHERE city = '" + city + "' ORDER BY AreaName ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                List<string> baranggays = new List<string>();
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    baranggays.Add(dr[0].ToString());
                }

                cb.ItemsSource = baranggays;
            }
        }
        public bool deduct_inventory(Booking_info book_info, Customer _customer, Receiver _receiver)
        {
            int stock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '"+book_info.item_name+"'"));
            if(stock >= int.Parse(book_info.quantity))
            {
                sql.Query($"UPDATE tbl_products SET unit_quantity = unit_quantity - '" + int.Parse(book_info.quantity) + "' WHERE item_name = '" + book_info.item_name + "'");
                return true;
                
            }
            else
            {
                return false;
            }
        }
        public void update_inventory_status(Booking_info book_info)
        {
            int newStock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '"+book_info.item_name+"'"));
            string Status = newStock < 0 ? Util.status_out_of_stock : newStock == 0 ? Util.status_out_of_stock : newStock <= 100 ? Util.status_low_stock : Util.status_in_stock;
            sql.Query($"UPDATE tbl_products set status = '"+Status+"' WHERE item_name = '"+book_info.item_name+"'");
            if (sql.HasException(true)) return;
        }
        public void load_dashboard(Label lbl_total_orders)
        {
            //for total orders
            sql.Query($"SELECT COUNT(*) FROM tbl_orders");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    lbl_total_orders.Content = dr[0].ToString();
                }
            }

            //for revenue
            sql.Query($"");
        }
    }
}
