using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    internal class SellingExpenses
    {
        decimal? adsBudget { get; set; }
        int? roas { get; set; }
        decimal adspentPerItem { get; set; }   
        decimal platformCommission { get; set; }
        decimal employeeCommission { get; set; }
        decimal shipping_fee { get; set; }
        decimal rts_margin { get; set; }
    }
}
