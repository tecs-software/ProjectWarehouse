using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.DeliverModule;
using WWarehouseManagement.Database;
using static WarehouseManagement.Views.Main.DeliverModule.DeliveryView;

namespace WarehouseManagement.Controller
{
    
    public class Show_order_inquiry
    {
        static sql_control sql = new sql_control();
        public static void show_inquiry_data(DataGrid dt)
        {
            sql.Query($"SELECT * FROM tbl_order_inquiry WHERE session_id = '{GlobalModel.session_id}' ORDER BY order_inquiry_id DESC");
            if(sql.DBDT.Rows.Count > 0)
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
