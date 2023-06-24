using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        sql_control sql = new sql_control();

        public void show_orders(DataGrid dg)
        {
            sql.Query($"SELECT * FROM tbl_orders");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
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
                        total = dr[8].ToString()

                        // Assign other properties as needed
                    };
                    orders.Add(order);

                }
                dg.ItemsSource = orders;
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

    }
}
