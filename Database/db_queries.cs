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
using System.Security.Cryptography;
using System.IO;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;
using WarehouseManagement.Views.Main.SystemSettingModule;
using System.Globalization;

namespace WarehouseManagement.Database
{
    public class db_queries
    {
        static sql_control sql = new sql_control();
        public bool insert_sender(string id,TextBox page_name, TextBox page_number, ComboBox cb_province, ComboBox cb_city, ComboBox cb_baranggay, TextBox address, string postal, int courier)
        {
            if(id == "0")
            {
                sql.AddParam("@id", int.Parse(id));
                sql.AddParam("@name", page_name.Text);
                sql.AddParam("@phone", page_number.Text);
                sql.AddParam("@province", cb_province.Text);
                sql.AddParam("@city", cb_city.Text);
                sql.AddParam("@baranggay", cb_baranggay.Text);
                sql.AddParam("@address", address.Text);
                sql.AddParam("@postal", postal);
                sql.AddParam("@courier", courier);

                sql.Query("EXEC SPadd_sender_info @id, @name, @province, @city, @baranggay, @phone, @address, @courier, @postal");
                return true;
            }
            else
            {
                sql.AddParam("@id", int.Parse(id));
                sql.AddParam("@name", page_name.Text);
                sql.AddParam("@phone", page_number.Text);
                sql.AddParam("@province", cb_province.Text);
                sql.AddParam("@city", cb_city.Text);
                sql.AddParam("@baranggay", cb_baranggay.Text);
                sql.AddParam("@address", address.Text);
                sql.AddParam("@postal", postal);
                sql.AddParam("@courier", courier);

                sql.Query("EXEC SPadd_sender_info @id, @name, @province, @city, @baranggay, @phone, @address, @courier, @postal");
                return true;
            }
        }
        public static bool checkExistingShop(string name)
        {
            sql.AddParam("@name", name);
            sql.Query($"SELECT * FROM tbl_sender WHERE sender_name = @name");
            if(sql.DBDT.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void UpdateCourier(string courierName,string customerId, string eCompanyId) =>
            sql.Query($"UPDATE tbl_couriers SET courier_name = '{courierName}', eccompany_id = '{eCompanyId}', customer_id = '{customerId}' ");
        public void PopulateShop(ComboBox cmb)
        {
            cmb.Items.Clear();
            sql.Query("SELECT sender_name FROM tbl_sender");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    cmb.Items.Add(dr[0].ToString());
                }
            }
            cmb.Items.Add("ADD");
        }
        public void DisplaySender(string name, SystemSettingPopup systemSetting)
        {
            sql.AddParam("@name", name);
            string barangay = "", city = "";
            sql.Query($"SELECT * FROM tbl_sender WHERE sender_name = @name ");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    systemSetting.txtId.Text = dr[0].ToString();
                    systemSetting.txtPagename.Text = dr[1].ToString();
                    systemSetting.txtPhone.Text = dr[5].ToString();
                    systemSetting.txtAddress.Text = dr[6].ToString();

                    systemSetting.cmbProvince.Text = dr[2].ToString();
                    barangay = dr[4].ToString();
                    city = dr[3].ToString();
                }
                systemSetting.cmbCity.Items.Clear();
                systemSetting.cmbBarangay.Items.Clear();

                sql.Query($"SELECT City, AreaName FROM tbl_address_delivery WHERE Province = '{systemSetting.cmbProvince.Text}' ");
                if (sql.HasException(true)) return;
                if(sql.DBDT.Rows.Count > 0)
                {
                    foreach(DataRow dr in sql.DBDT.Rows)
                    {
                        systemSetting.cmbCity.Items.Add(dr[0].ToString());
                        systemSetting.cmbBarangay.Items.Add(dr[1].ToString());
                    }
                    systemSetting.cmbCity.Text = city;
                    systemSetting.cmbBarangay.Text = barangay;
                }
            }
        }
        public static void get_sender()
        {
            int? sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_users WHERE user_id = {CurrentUser.Instance.userID}"));
            sql.Query($"SELECT * FROM tbl_sender WHERE sender_id = {sender_id}");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    GlobalModel.sender_id = int.Parse(dr[0].ToString());
                    GlobalModel.sender_name = dr[1].ToString();
                    GlobalModel.sender_province = dr[2].ToString();
                    GlobalModel.sender_city = dr[3].ToString();
                    GlobalModel.sender_area = dr[4].ToString();
                    GlobalModel.sender_phone = dr[5].ToString();
                    GlobalModel.sender_address = dr[6].ToString();
                }
            }

            sql.Query($"SELECT * FROM tbl_couriers");
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    GlobalModel.key = dr[2].ToString();
                    GlobalModel.eccompany_id = dr[3].ToString();
                    GlobalModel.customer_id = dr[4].ToString();
                }
            }
        }
        public void api_credentials(RadioButton courier, string api_key, string ec, TextBox customer_id)
        {
            sql.Query($"INSERT INTO tbl_couriers (courier_name, api_key, eccompany_id, customer_id) " +
                $"VALUES ('{courier.Content}','{Encrypt(api_key)}' ,'{ec}', '{customer_id.Text}')");
            if (sql.HasException(true)) return;
        }
        public void insert_receiver(Receiver _receiver)
        {
            string name = _receiver.FirstName + " " + _receiver.LastName;
            sql.AddParam("@name", name);
            sql.AddParam("@address", _receiver.Address);
            sql.Query($"INSERT INTO tbl_receiver (receiver_name, receiver_phone, receiver_address) VALUES (@name, '" + _receiver.Phone + "', @address)");
            if (sql.HasException(true)) return;
        }
        public void Insert_Orders(string order_id, string waybill, Booking_info book_info, string status)
        {
            //string? product_id = string.Empty;

            //sql.Query($"SELECT product_id FROM tbl_product WHERE product_name = @select_product ");
            //if(sql.HasException(true))
            //if (sql.DBDT.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in sql.DBDT.Rows)
            //    {
            //        product_id = dr[0].ToString();
            //        MessageBox.Show("dr" + dr[0].ToString());
            //    }
            //}

            sql.AddParam("@select_product", book_info.item_name);
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name = @select_product ");

            string receiver_id = sql.ReturnResult($"SELECT TOP 1(receiver_id) FROM tbl_receiver ORDER BY receiver_id DESC");

            string sender_id = sql.ReturnResult($"SELECT sender_id FROM tbl_products WHERE product_id = '{product_id}'");

            decimal total = book_info.cod;

            sql.AddParam("@remarks", book_info.remarks);

            //dito papalitan couriers
            sql.Query($"INSERT INTO tbl_orders (order_id, waybill_number, user_id, sender_id, receiver_id, product_id, quantity, total, remarks, status, created_at, updated_at, courier) VALUES " +
                $"('{order_id}', '{waybill}', '{CurrentUser.Instance.userID.ToString()}', '{sender_id}', '{receiver_id}', '{product_id}', {book_info.quantity}, {total}, @remarks, '{status}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 'J&T' )");
            if (sql.HasException(true)) return;
        }

        public void insert_Incentives(Booking_info book_Info, string order_id)
        {
            sql.AddParam("@item_name", book_Info.item_name);
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name = @item_name");
            sql.Query($"SELECT employee_commission FROM tbl_selling_expenses WHERE product_id = '{product_id}'");
            if(sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                   decimal commisions = decimal.Parse(dr[0].ToString());

                    decimal total_commi = commisions * decimal.Parse(book_Info.quantity);

                    sql.Query($"INSERT INTO tbl_incentives (user_id, incentive_for, quantity, total_incentive, is_valid) VALUES " +
                        $"({CurrentUser.Instance.userID}, '{order_id}', {book_Info.quantity}, {total_commi}, 1)");
                    if (sql.HasException(true)) return;

                }
            }
        }
        #region J&T addressing
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
        public async void city(ComboBox cb, string province)
        {
            cb.Items.Clear();
            sql.Query($"SELECT distinct city FROM tbl_address_delivery WHERE province = '{province}' AND CanDeliver = '1' ORDER BY city ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    cb.Items.Add(dr[0].ToString());
                }
            }
        }
        public void baranggay(ComboBox cb, string city)
        {
            cb.Items.Clear();
            sql.Query($"SELECT distinct AreaName FROM tbl_address_delivery WHERE city = '{city}' ORDER BY AreaName ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    cb.Items.Add(dr[0].ToString());
                }

            }
        }
        #endregion

        #region FlashAddress
        public void FlashProvince(ComboBox cb)
        {
            sql.Query($"SELECT distinct province FROM tbl_flashAddressing ORDER BY Province ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                List<string> provinces = new List<string>();
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    provinces.Add(dr[0].ToString());
                }

                cb.ItemsSource = provinces;
            }
        }
        public void FlashCity(ComboBox cb, string province)
        {
            cb.Items.Clear();
            sql.Query($"SELECT distinct City FROM tbl_flashAddressing WHERE Province = '{province}' ORDER BY City ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    cb.Items.Add(dr[0].ToString());
                }
            }
        }
        public void FlashBaranggay(ComboBox cb, string city)
        {
            cb.Items.Clear();
            sql.Query($"SELECT distinct Barangay FROM tbl_flashAddressing WHERE city = '{city}' ORDER BY Barangay ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    cb.Items.Add(dr[0].ToString());
                }

            }
        }
        public void FlashPostalCode(ComboBox cb, string barangay)
        {
            cb.Items.Clear();
            sql.Query($"SELECT distinct PostalCode FROM tbl_flashAddressing WHERE Barangay = '{barangay}' ORDER BY PostalCode ASC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    cb.Items.Add(dr[0].ToString());
                }

            }
        }
        #endregion
        public bool ValidateSenderName(string name, int id)
        {
            sql.Query($"SELECT * FROM tbl_sender WHERE sender_name = '{name}' ");
            if (sql.DBDT.Rows.Count > 0)
                return false;
            else
                return true;
        }
        
        public bool check_quantity(Booking_info book_info, Receiver _receiver)
        {
            int stock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '"+book_info.item_name+"'"));
            if(stock >= int.Parse(book_info.quantity))
            {
                return true;
                
            }
            else
            {
                return false;
            }
        }
        public void update_inventory_status(Booking_info book_info)
        {
            sql.AddParam("@item_name", book_info.item_name);

            //deducting the ordered quantity
            sql.Query($"UPDATE tbl_products SET unit_quantity = unit_quantity - {int.Parse(book_info.quantity)} WHERE item_name = @item_name");
            if (sql.HasException(true)) return;

            //updating status
            int newStock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '{book_info.item_name}'"));
            string Status = newStock < 0 ? Util.status_out_of_stock : newStock == 0 ? Util.status_out_of_stock : newStock <= 100 ? Util.status_low_stock : Util.status_in_stock;
            sql.Query($"UPDATE tbl_products set status = '{Status}' WHERE item_name = '{book_info.item_name}'");
            if (sql.HasException(true)) return;
        }
        public async Task load_dashboard_summary(Label lbl_total_orders, Label lbl_gross, Label lbl_products_sold, Label net_profit, int days)
        {
            DateTime start_time = DateTime.Now.AddDays(-days).Date;
            DateTime end_time = DateTime.Now.AddDays(1).Date;

            sql.AddParam("@startTime", start_time);
            sql.AddParam("@endTime", end_time);

            //for total orders
            sql.Query($"SELECT COUNT(*) FROM tbl_orders WHERE status != 'CANCELLED' AND created_at BETWEEN @startTime AND @endTime");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    lbl_total_orders.Content = dr[0].ToString();
                }
            }

            sql.AddParam("@startTime", start_time);
            sql.AddParam("@endTime", end_time);

            //for gross sales
            sql.Query($"SELECT COALESCE(SUM(total),0) FROM tbl_orders WHERE status = 'DELIVERED' AND updated_at BETWEEN @startTime AND @endTime");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    lbl_gross.Content = dr[0].ToString();
                }
            }

            sql.AddParam("@startTime", start_time);
            sql.AddParam("@endTime", end_time);
            //for total projected sales
            sql.Query($"SELECT COALESCE(SUM(total),0) FROM tbl_orders WHERE status != 'CANCELLED' AND created_at BETWEEN @startTime AND @endTime");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    lbl_products_sold.Content = dr[0].ToString();
                }
            }

            sql.AddParam("@startTime", start_time);
            sql.AddParam("@endTime", end_time);
            //for netprofit
            decimal total_sales = decimal.Parse(sql.ReturnResult($"SELECT COALESCE(SUM(total), 0) FROM tbl_orders WHERE status = 'DELIVERED' AND updated_at BETWEEN @startTime AND @endTime"));
            
            sql.AddParam("@startTime", start_time);
            sql.AddParam("@endTime", end_time);
            decimal total_expenses = decimal.Parse(sql.ReturnResult($"SELECT COALESCE(SUM(AdSpent + Utilities + Miscellaneous), 0) FROM tbl_expenses WHERE Date BETWEEN @startTime AND @endTime"));

            net_profit.Content = total_sales - total_expenses;

        }
        public bool checkAddress()
        {
            int count = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_flashAddressing"));
            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkJTAddress()
        {
            int count = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_address_delivery"));
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task sales_graph(int days, CartesianChart chart)
        {
            ChartValues<ObservableValue> revenueData = new ChartValues<ObservableValue>();
            List<string> dateList = new List<string>();
            string formattedNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            DateTime start_time = DateTime.ParseExact(formattedNow, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture).AddDays(-days).Date;
            DateTime end_time = DateTime.ParseExact(formattedNow, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture).AddDays(1).Date;
            //for revenue
            sql.AddParam("@start_time", start_time);
            sql.AddParam("@end_time", end_time);
            sql.Query($"SELECT COALESCE(SUM(total),0), updated_at FROM tbl_orders WHERE status = 'DELIVERED' AND updated_at BETWEEN @start_time AND @end_time GROUP BY updated_at");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    revenueData.Add(new ObservableValue(double.Parse(dr[0].ToString())));
                    dateList.Add(DateTime.Parse(dr[1].ToString()).ToString());
                }

                chart.AxisX.Clear();
                chart.AxisX.Add(new Axis
                {
                    Title = "Date",
                    Labels = dateList
                });

                chart.Series.Clear();
                chart.Series.Add(new LineSeries
                {
                    Title = "Total Revenue",
                    Values = revenueData,
                });
            }
            else
            {
                chart.Series.Clear();
                chart.AxisX.Clear();
            }
        }
        public string Encrypt(string input)
        {
            string key = "YourEncryptionKey"; // Replace with your desired encryption key

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                inputBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(inputBytes);
        }
    }
}
