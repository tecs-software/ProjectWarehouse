using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class Booking_info
    {
        public string item_name { get; set; }
        public string weight { get; set; } = string.Empty;  
        public string quantity { get; set; } = string.Empty;  
        public string goods_value { get; set; } = string.Empty;
        public string bag_specification { get; set; } = string.Empty;
        public string remarks { get; set; }
        public string courier { get; set; } = "J&T";
        public decimal cod { get; set; } = 0;
    }
}
