using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class update_order_status
    {
        sql_control sql = new sql_control();

        public void get_order_status(string waybill)
        {
            string? status = sql.ReturnResult($"SELECT TOP 1 scan_type FROM tbl_status WHERE waybill# = {waybill}");
            if (sql.HasException(true)) return;
            string? date_updated = sql.ReturnResult($"SELECT TOP 1 scan_time FROM tbl_status WHERE waybill# = {waybill}");
            if (sql.HasException(true)) return;

            if(status != null || date_updated != null)
                sql.Query($"UPDATE tbl_orders SET updated_at = '{DateTime.Parse(date_updated)}', status = '{status.ToUpper()}' WHERE waybill_number = {waybill}");
            if (sql.HasException(true)) return;
            else
            {
                //do nothing
            }
        }
    }
}
