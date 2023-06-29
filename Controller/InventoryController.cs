using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class InventoryController
    {
        static sql_control sql = new sql_control();
        public static void LoadSender(string? productId, ComboBox cmb)
        {
            cmb.Text = sql.ReturnResult($"SELECT sender_name FROM tbl_products LEFT JOIN tbl_sender ON tbl_products.sender_id = tbl_sender.sender_id WHERE product_id = '{productId}' ");
            if (sql.HasException(true)) return;
        }
    }
}
