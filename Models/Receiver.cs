using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class Receiver
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; } 
        public string MiddleName { get; set; }
        public string LastName { get; set; }     
        public string Phone { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Barangay { get; set; }
        public string Address { get; set; }
    }
}
