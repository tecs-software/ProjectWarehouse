using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class Expenses
    {
        public int ID { get; set; }
        public int? UserID { get; set; }
        public decimal AdSpent { get; set; }
        public decimal Utilities { get; set; }
        public decimal Miscellaneous { get; set; }
        public DateTime Date { get; set; }

    }
}
