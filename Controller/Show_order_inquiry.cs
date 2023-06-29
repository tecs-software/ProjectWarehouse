using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    
    public class Show_order_inquiry
    {
        sql_control sql = new sql_control();
        public void show_inquiry_data(DataGrid dt)
        {
            sql.Query($"SELECT * FROM tbl_order_inquiry");
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
                        remarks = dr[8].ToString()
                    };
                    parcel_details.Add(parcel);
                }
                dt.ItemsSource = parcel_details;
            }
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
        }
    }
}
