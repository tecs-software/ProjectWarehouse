using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WarehouseManagement.Database
{
    public class FlashDB
    {
        static sql_control sql = new sql_control();

        public static void ReceiverData(FLASHModel model)
        {
            string name = model.receiver_name;
            sql.AddParam("@name", name);
            sql.AddParam("@address", model.receiver_address.Replace("'",""));
            sql.Query($"INSERT INTO tbl_receiver (receiver_name, receiver_phone, receiver_address) VALUES (@name, '{model.receiver_phone}', @address)");
            if (sql.HasException(true)) return;
        }
        public static void OrderData(FLASHModel model, string order_id, string waybill)
        {
            sql.AddParam("@select_product", model.item);
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name = @select_product ");

            string receiver_id = sql.ReturnResult($"SELECT TOP 1(receiver_id) FROM tbl_receiver ORDER BY receiver_id DESC");

            string sender_id = sql.ReturnResult($"SELECT sender_id FROM tbl_products WHERE product_id = '{product_id}'");

            decimal total = decimal.Parse(model.COD);

            sql.AddParam("@remarks", model.remarks.Replace("'",""));

            //dito papalitan couriers
            sql.Query($"INSERT INTO tbl_orders (order_id, waybill_number, user_id, sender_id, receiver_id, product_id, quantity, total, remarks, status, created_at, updated_at, courier) VALUES " +
                $"('{order_id}', '{waybill}', '{CurrentUser.Instance.userID.ToString()}', '{sender_id}', '{receiver_id}', '{product_id}', 1, {total}, @remarks, 'PENDING', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 'FLASH' )");
            if (sql.HasException(true)) return;
        }
        public static void UpdateStocks(FLASHModel model)
        {
            sql.AddParam("@item_name", model.item);

            //deducting the ordered quantity
            sql.Query($"UPDATE tbl_products SET unit_quantity = unit_quantity - 1 WHERE item_name = @item_name");
            if (sql.HasException(true)) return;

            //updating status
            int newStock = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '{model.item}'"));
            string Status = newStock < 0 ? Util.status_out_of_stock : newStock == 0 ? Util.status_out_of_stock : newStock <= 100 ? Util.status_low_stock : Util.status_in_stock;
            sql.Query($"UPDATE tbl_products set status = '{Status}' WHERE item_name = '{model.item}'");
            if (sql.HasException(true)) return;
        }
        public static void UpdateCancelledOrder(string reason, string product, string id)
        {
            sql.Query($"UPDATE tbl_orders SET remarks = '{reason}', status = 'CANCELLED', updated_at = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE order_id = '{id}'");

            int order_qty = int.Parse(sql.ReturnResult($"SELECT quantity FROM tbl_orders WHERE order_id = '{id}'"));
            sql.Query($"UPDATE tbl_products SET unit_quantity = unit_quantity+{order_qty} WHERE item_name = '{product}'");

            int stocks = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '{product}'"));
            string status = stocks < 0 ? Util.status_out_of_stock : (stocks == 0 ? Util.status_out_of_stock : (stocks <= 100 ? Util.status_low_stock : Util.status_in_stock));
            sql.Query($"UPDATE tbl_products SET status = '{status}' WHERE item_name = '{product}'");

            //invalidating the incentives
            sql.Query($"UPDATE tbl_incentives SET is_valid = 0 WHERE incentive_for = '{id}'");
        }
    }
}
