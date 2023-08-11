using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    
    public class show_DT
    {
        static sql_control sql = new sql_control();
        public bool exceedResult { get; set; } = false;
        public async Task show_orders(DataGrid dg, int offsetCount) 
        {
            if (CurrentUser.Instance.userID == 1)
            {
                sql.Query($"SELECT * FROM tbl_orders WHERE status != 'FAILED' ORDER BY created_at DESC OFFSET {offsetCount} ROWS FETCH NEXT 12 ROWS ONLY;");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    List<Orders> orders = new List<Orders>();
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        Orders order = new Orders
                        {
                            // Assign values from the DataRow to the properties of the Order object

                            ID = dr[0].ToString(),
                            Waybill = dr[2].ToString(),
                            status = dr[10].ToString(),
                            customer_name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                            address = sql.ReturnResult($"SELECT receiver_address FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                            product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '" + dr[6].ToString() + "'"),
                            courier = dr[1].ToString(),
                            quantity = dr[7].ToString(),
                            total = dr[8].ToString(),
                            date_created = DateTime.Parse(dr[11].ToString()).ToString("MMM-dd-yyyy HH:mm:ss tt")

                            // Assign other properties as needed
                        };
                        orders.Add(order);

                    }
                    dg.ItemsSource = orders;
                    dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    exceedResult = false;
                }
                else
                {
                    exceedResult = true;
                }
            }
            else
            {
                sql.Query($"SELECT * FROM tbl_orders WHERE user_id = {int.Parse(CurrentUser.Instance.userID.ToString())} ORDER BY created_at DESC");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    List<Orders> orders = new List<Orders>();
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        Orders order = new Orders
                        {
                            // Assign values from the DataRow to the properties of the Order object

                            ID = dr[0].ToString(),
                            Waybill = dr[2].ToString(),
                            status = dr[10].ToString(),
                            customer_name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                            address = sql.ReturnResult($"SELECT receiver_address FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                            product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '" + dr[6].ToString() + "'"),
                            courier = dr[1].ToString(),
                            quantity = dr[7].ToString(),
                            total = dr[8].ToString(),
                            date_created = DateTime.Parse(dr[11].ToString()).ToString("MMM-dd-yyyy HH:mm:ss tt")

                            // Assign other properties as needed
                        };
                        orders.Add(order);

                    }
                    dg.ItemsSource = orders;
                    dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
            }
        }
        public static void search_orders_data(TextBox tb_search_bar, RadioButton rbtn_waybill, RadioButton cs_name, DataGrid dg)
        {
            dg.ItemsSource = null;
            dg.Items.Clear();
            if (CurrentUser.Instance.userID == 1)
            {
                if (rbtn_waybill.IsChecked == true)
                {
                    sql.Query($"SELECT * FROM tbl_orders WHERE waybill_number = '{tb_search_bar.Text}' ORDER BY created_at DESC");
                    if (sql.HasException(true)) return;
                    if (sql.DBDT.Rows.Count > 0)
                    {
                        List<Orders> orders = new List<Orders>();
                        foreach (DataRow dr in sql.DBDT.Rows)
                        {
                            Orders order = new Orders
                            {
                                // Assign values from the DataRow to the properties of the Order object

                                ID = dr[0].ToString(),
                                Waybill = dr[2].ToString(),
                                status = dr[10].ToString(),
                                customer_name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                                address = sql.ReturnResult($"SELECT receiver_address FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                                product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '" + dr[6].ToString() + "'"),
                                courier = dr[1].ToString(),
                                quantity = dr[7].ToString(),
                                total = dr[8].ToString(),
                                date_created = DateTime.Parse(dr[11].ToString()).ToString("MMM-dd-yyyy HH:mm:ss tt")

                                // Assign other properties as needed
                            };
                            orders.Add(order);

                        }
                        dg.ItemsSource = orders;
                        dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    }
                }
                else if (cs_name.IsChecked == true)
                {
                    sql.Query($"SELECT receiver_id FROM tbl_receiver WHERE receiver_name = '{tb_search_bar.Text}'");
                    if (sql.HasException(true)) return;
                    if (sql.DBDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in sql.DBDT.Rows)
                        {
                            if (dr[0].ToString() == "" || dr[0] == null)
                            {

                            }
                            else
                            {
                                sql.Query($"SELECT * FROM tbl_orders WHERE receiver_id = {int.Parse(dr[0].ToString())} ORDER BY created_at DESC");
                                if (sql.HasException(true)) return;
                                if (sql.DBDT.Rows.Count > 0)
                                {
                                    List<Orders> orders = new List<Orders>();
                                    foreach (DataRow dr1 in sql.DBDT.Rows)
                                    {
                                        Orders order = new Orders
                                        {
                                            // Assign values from the DataRow to the properties of the Order object

                                            ID = dr1[0].ToString(),
                                            Waybill = dr1[2].ToString(),
                                            status = dr1[10].ToString(),
                                            customer_name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = '" + dr1[5].ToString() + "'"),
                                            address = sql.ReturnResult($"SELECT receiver_address FROM tbl_receiver WHERE receiver_id = '" + dr1[5].ToString() + "'"),
                                            product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '" + dr1[6].ToString() + "'"),
                                            courier = dr1[1].ToString(),
                                            quantity = dr1[7].ToString(),
                                            total = dr1[8].ToString(),
                                            date_created = DateTime.Parse(dr1[11].ToString()).ToString("MMM-dd-yyyy HH:mm:ss tt")

                                            // Assign other properties as needed
                                        };
                                        orders.Add(order);

                                    }
                                    dg.ItemsSource = orders;
                                    dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (rbtn_waybill.IsChecked == true)
                {
                    sql.Query($"SELECT * FROM tbl_orders WHERE waybill_number = '{tb_search_bar.Text}' AND user_id = {int.Parse(CurrentUser.Instance.userID.ToString())} ORDER BY created_at DESC");
                    if (sql.HasException(true)) return;
                    if (sql.DBDT.Rows.Count > 0)
                    {
                        List<Orders> orders = new List<Orders>();
                        foreach (DataRow dr in sql.DBDT.Rows)
                        {
                            Orders order = new Orders
                            {
                                // Assign values from the DataRow to the properties of the Order object

                                ID = dr[0].ToString(),
                                Waybill = dr[2].ToString(),
                                status = dr[10].ToString(),
                                customer_name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                                address = sql.ReturnResult($"SELECT receiver_address FROM tbl_receiver WHERE receiver_id = '" + dr[5].ToString() + "'"),
                                product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '" + dr[6].ToString() + "'"),
                                courier = dr[1].ToString(),
                                quantity = dr[7].ToString(),
                                total = dr[8].ToString(),
                                date_created = DateTime.Parse(dr[11].ToString()).ToString("MMM-dd-yyyy HH:mm:ss tt")

                                // Assign other properties as needed
                            };
                            orders.Add(order);

                        }
                        dg.ItemsSource = orders;
                        dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    }
                }
                else if (cs_name.IsChecked == true)
                {
                    sql.Query($"SELECT receiver_id FROM tbl_receiver WHERE receiver_name = '{tb_search_bar.Text}'");
                    if (sql.HasException(true)) return;
                    if (sql.DBDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in sql.DBDT.Rows)
                        {
                            if (dr[0].ToString() == "" || dr[0] == null)
                            {

                            }
                            else
                            {
                                sql.Query($"SELECT * FROM tbl_orders WHERE receiver_id = {int.Parse(dr[0].ToString())} AND user_id = {int.Parse(CurrentUser.Instance.userID.ToString())} ORDER BY created_at DESC");
                                if (sql.HasException(true)) return;
                                if (sql.DBDT.Rows.Count > 0)
                                {
                                    List<Orders> orders = new List<Orders>();
                                    foreach (DataRow dr1 in sql.DBDT.Rows)
                                    {
                                        Orders order = new Orders
                                        {
                                            // Assign values from the DataRow to the properties of the Order object

                                            ID = dr1[0].ToString(),
                                            Waybill = dr1[2].ToString(),
                                            status = dr1[10].ToString(),
                                            customer_name = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = '" + dr1[5].ToString() + "'"),
                                            address = sql.ReturnResult($"SELECT receiver_address FROM tbl_receiver WHERE receiver_id = '" + dr1[5].ToString() + "'"),
                                            product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '" + dr1[6].ToString() + "'"),
                                            courier = dr1[1].ToString(),
                                            quantity = dr1[7].ToString(),
                                            total = dr1[8].ToString(),
                                            date_created = DateTime.Parse(dr1[11].ToString()).ToString("MMM-dd-yyyy HH:mm:ss tt")

                                            // Assign other properties as needed
                                        };
                                        orders.Add(order);

                                    }
                                    dg.ItemsSource = orders;
                                    dg.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
    public class Orders
    {
        public string ID { get; set; }
        public string Waybill { get; set; }
        public string status { get; set; }
        public string customer_name { get; set; }
        public string address { get; set; } 
        public string product { get; set; }
        public string courier { get; set; }
        public string quantity { get; set; }
        public string total { get; set; }
        public string date_created { get; set; }

    }
}
