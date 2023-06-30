using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWarehouseManagement.Database;
using System.Windows;
using System.Data;

namespace WarehouseManagement.Controller
{
    public class insert_profits
    {
        sql_control sql = new sql_control();
        public void insert_profit(string product, decimal expenses, decimal net) 
        {
            string product_id = sql.ReturnResult($"SELECT product_id FROM tbl_products WHERE item_name = '{product}'");

            sql.Query($"UPDATE tbl_selling_expenses SET total_expenses = {expenses}, net_profit = {net} WHERE product_id = '{product_id}'");
            if (sql.HasException(true)) return;
        }

    }
}
