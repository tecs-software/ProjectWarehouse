using LiveCharts.Defaults;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
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
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using WWarehouseManagement.Database;
using System.Collections.Immutable;

namespace WarehouseManagement.Database
{
    public class db_queries
    {
        sql_control sql = new sql_control();
        public bool insert_sender(TextBox page_name, TextBox page_number, ComboBox cb_province, ComboBox cb_city, ComboBox cb_baranggay, TextBox address)
        {
            sql.AddParam("@name", page_name.Text);
            sql.AddParam("@phone", page_number.Text);
            sql.AddParam("@province", cb_province.Text);
            sql.AddParam("@city", cb_city.Text);
            sql.AddParam("@baranggay", cb_baranggay.Text);
            sql.AddParam("@address", address.Text);

            sql.Query("EXEC SPadd_sender_info @name, @province, @city, @baranggay, @phone, @address");
            int count = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_sender"));
            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public void get_sender(GlobalModel sender)
        {
            sql.Query($"SELECT * FROM tbl_sender");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    sender.sender_name = dr[1].ToString();
                    sender.sender_province = dr[2].ToString();
                    sender.sender_city = dr[3].ToString();
                    sender.sender_area = dr[4].ToString();
                    sender.sender_phone = dr[5].ToString();
                    sender.sender_address = dr[6].ToString();
                }
            }

            sql.Query($"SELECT * FROM tbl_couriers");
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    sender.key = dr[2].ToString();
                    sender.eccompany_id = dr[3].ToString();
                    sender.customer_id = dr[4].ToString();
                }
            }
        }
        public void api_credentials(ComboBox courier, TextBox api_key, TextBox ec, TextBox customer_id)
        {
            sql.Query($"INSERT INTO tbl_couriers (courier_name, api_key, eccompany_id, customer_id) VALUES" +
                $"('"+courier.Text+"', '"+api_key.Text+"', '"+ec.Text+"', '"+customer_id.Text+"')");
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
                "'Pending', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','J&T')");
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
        public bool deduct_inventory(Booking_info book_info, Receiver _receiver)
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
        public void load_dashboard_summary(Label lbl_total_orders, Label lbl_gross, Label lbl_products_sold, Label lbl_expenses, Label net_profit)
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

            //for gross sales
            sql.Query($"SELECT COALESCE(SUM(total),0) FROM tbl_orders WHERE status = 'Delivered'");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    lbl_gross.Content = dr[0].ToString();
                }
            }

            //for total projected sales
            sql.Query($"SELECT COALESCE(SUM(total),0) FROM tbl_orders");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    lbl_products_sold.Content = dr[0].ToString();
                }
            }

            //for expenses
            sql.Query($"SELECT COALESCE(SUM(total_expenses), 0) FROM tbl_selling_expenses");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    lbl_expenses.Content = decimal.Parse(dr[0].ToString());
                    
                }
            }

            //for net profit
            decimal netprofit = decimal.Parse(sql.ReturnResult($"SELECT COALESCE(SUM(net_profit), 0) FROM tbl_selling_expenses"));
            net_profit.Content = netprofit;

        }
        public bool check_sender_info()
        {
            int count = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_sender"));
            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void sales_graph(DatePicker start, DatePicker end, CartesianChart chart)
        {
            ChartValues<ObservableValue> revenueData = new ChartValues<ObservableValue>();
            List<DateTime> dateList = new List<DateTime>();

            //for revenue
            sql.Query($"SELECT COALESCE(SUM(total),0), updated_at FROM tbl_orders WHERE status = 'Delivered' AND updated_at BETWEEN '" + start + "' AND '" + end + "' GROUP BY updated_at");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    revenueData.Add(new ObservableValue(double.Parse(dr[0].ToString())));
                    dateList.Add(DateTime.Parse(dr[1].ToString()));
                }
                List<string> dateLabels = dateList.Select(date => date.ToString("dd/MM/yyyy")).ToList();

                chart.AxisX.Clear();
                chart.AxisX.Add(new Axis
                {
                    Title = "Date",
                    Labels = dateLabels
                });

                chart.Series.Clear();
                chart.Series.Add(new LineSeries
                {
                    Title = "Total Revenue",
                    Values = revenueData,
                });
            }
        }
        public bool check_addresses()
        {
            int addreses = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_address_delivery"));
            if(addreses > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool check_waybill(TextBox waybill)
        {
            sql.Query($"SELECT waybill# FROM tbl_order_inquiry");
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    if (waybill.Text == dr[0].ToString())
                    {
                        
                    }
                        
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
