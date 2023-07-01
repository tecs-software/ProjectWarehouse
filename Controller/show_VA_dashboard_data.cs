using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;
using System.Windows;

namespace WarehouseManagement.Controller
{
    public class show_VA_dashboard_data
    {
        sql_control sql = new sql_control();
        
        public void show_VA_data(System.Windows.Controls.Label label)
        {
            decimal commisions = decimal.Parse(sql.ReturnResult($"SELECT COALESCE(SUM(tbl_incentives.total_incentive),0) FROM tbl_incentives " +
                $"INNER JOIN tbl_orders ON tbl_incentives.incentive_for = tbl_orders.order_id " +
                $"WHERE tbl_orders.status != 'CANCELLED' AND tbl_incentives.issued != 1 AND tbl_incentives.user_id = {CurrentUser.Instance.userID}"));
            if (sql.HasException(true)) return;

            label.Content = commisions.ToString();
        }
    }
}
