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
    internal class Order_Controller
    {
        static sql_control sql = new sql_control();
        public static string id { get; set; }
        public static bool isConfirmedToReturn { get; set; } = false;
        public static bool isBarcodeExist(string barcode)
        {
            sql.Query($"SELECT order_id FROM tbl_orders WHERE waybill_number = '{barcode}' ");
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                     id = dr[0].ToString();
                }
                return true;
            }
            else
                return false;
        }
        public static void UpdateStatus(string id) => sql.Query($"UPDATE tbl_orders SET status = 'Returned to Sender' WHERE order_id = '{id}' ");
    }
}
