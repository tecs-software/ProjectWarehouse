using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.InventoryModule.CustomDialogs;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class bulk_inserts
    {
        static sql_control sql = new sql_control();
        

        public static void bulk_receiver(bulk_model model) => sql.Query($"EXEC SPadd_receiver '{model.receiver_name}', '{model.receiver_phone}', '{model.receiver_address}'");
        public static void bulk_orders(bulk_model model, string waybill, string order_id) => sql.Query($"EXEC SPadd_orders '{order_id}', 'J&T', '{waybill}', {CurrentUser.Instance.userID}, '{model.product_name}'," +
            $"{model.quantity}, {model.total}, '{model.remarks}', 'PENDING', '{model.receiver_phone}', '{model.receiver_address}'");
        public static void bulk_incentives(bulk_model model, string order_id) => sql.Query($"EXEC SPadd_incentives {CurrentUser.Instance.userID}, '{order_id}', {model.quantity}," +
            $"{1}, '{model.product_name}'");
        public static void bulk_update_quantity(bulk_model model) => sql.Query($"EXEC SPupdate_stocks {model.quantity}, '{model.product_name}'");
        public static void bulk_update_stocks(bulk_model model)
        {
            int newStock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '{model.product_name}'"));
            string Status = newStock < 0 ? Util.status_out_of_stock : newStock == 0 ? Util.status_out_of_stock : newStock <= 100 ? Util.status_low_stock : Util.status_in_stock;
            sql.Query($"UPDATE tbl_products set status = '{Status}' WHERE item_name = '{model.product_name}'");
            if (sql.HasException(true)) return;
        }
    }
}
