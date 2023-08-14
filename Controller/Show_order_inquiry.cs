using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.DeliverModule;
using WarehouseManagement.Views.Main.OrderModule;
using WWarehouseManagement.Database;
using static WarehouseManagement.Views.Main.DeliverModule.DeliveryView;

namespace WarehouseManagement.Controller
{
    
    public class Show_order_inquiry
    {
        static sql_control sql = new sql_control();
        public static bool exceedResult { get; set; } = false;

        public static void show_inquiry_data(DataGrid dt, bool clickedNext)
        {
            int result_count;
            if (clickedNext)
            {
                if (!exceedResult)
                {
                    DeliveryTable.offsetCount = DeliveryTable.offsetCount + 12;
                }
            }
            else
            {
                if (DeliveryTable.offsetCount == 0)
                    DeliveryTable.offsetCount = 0;
                else
                {
                    DeliveryTable.offsetCount = DeliveryTable.offsetCount - 12;
                    exceedResult = false;
                }

            }
            sql.Query($"SELECT * FROM tbl_order_inquiry WHERE session_id = '{GlobalModel.session_id}' ORDER BY order_inquiry_id DESC OFFSET {DeliveryTable.offsetCount} ROWS FETCH NEXT 12 ROWS ONLY;");
            result_count = sql.DBDT.Rows.Count;
            if (sql.DBDT.Rows.Count > 0)
            {
                List<parcel_data> parcel_details = new List<parcel_data>();
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    parcel_data parcel = new parcel_data
                    {
                        waybill = dr[1].ToString(),
                        receiver = dr[2].ToString(),
                        contact = dr[3].ToString(),
                        address = dr[4].ToString(),
                        product = dr[5].ToString(),
                        qty = dr[6].ToString(),
                        weight = dr[7].ToString(),
                        remarks = dr[8].ToString(),
                        date = dr[9].ToString()
                    };
                    parcel_details.Add(parcel);
                }
                dt.ItemsSource = parcel_details;
                exceedResult = false;

                if (clickedNext)
                {
                    if (result_count < 11)
                    {
                        exceedResult = true;
                        return;
                    }

                }
            }
            else
            {
                dt.ItemsSource = null;
            }
        }
        public static void soft_delete(string waybill)
        {
            sql.Query($"DELETE FROM tbl_order_inquiry WHERE waybill# = '{waybill}'");
            if (sql.HasException(true)) return;
        }
        public class parcel_data
        {
            public string waybill { get; set; }
            public string receiver { get; set;}
            public string contact { get; set;}
            public string address { get; set;}
            public string product { get; set;}
            public string qty { get; set; }
            public string weight { get; set; }
            public string remarks { get; set; }
            public string date { get; set; }
        }
    }
}
