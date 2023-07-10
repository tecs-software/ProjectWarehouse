using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class bulk_model
    {
        // receivers
        public string receiver_name { get; set; }
        public string receiver_phone { get; set; }
        public string receiver_address { get; set; }
        public string receiver_province { get; set; }
        public string receiver_city { get; set; }
        public string receiver_area { get; set; }


        // other fields
        public string remarks { get; set; }
        public string product_name { get; set; } //items
        public decimal total { get; set; }
        public int quantity { get; set; }
        public int weight { get; set; }

        //etc
        public decimal parcel_value { get; set; }
        public decimal cod { get; set; }
        public string parcel_name { get; set; }
        public int total_parcel { get; set; }
    }
}
