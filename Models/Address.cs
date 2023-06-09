using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    internal class Address
    {
        public class Province
        {
            public int province_id { get; set; }
            public int region_id { get; set; }
            public string province_name { get; set; }
        }

        public class Municipality
        {
            public int municipality_id { get; set; }
            public int province_id { get; set; }
            public string municipality_name { get; set; }
        }

        public class Barangay
        {
            public int barangay_id { get; set; }
            public int municipality_id { get; set; }
            public string barangay_name { get; set; }
        }
    }
}
