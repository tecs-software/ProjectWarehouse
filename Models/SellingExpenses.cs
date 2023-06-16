using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class SellingExpenses
    {
        public decimal? adsBudget { get; set; }
        public int? roas { get; set; }
        public decimal? adspentPerItem { get; set; }
        public decimal? platformCommission { get; set; }
        public decimal? employeeCommission { get; set; }
        public decimal? shippingFee { get; set; }
        public decimal? rtsMargin { get; set; }
    }
}
