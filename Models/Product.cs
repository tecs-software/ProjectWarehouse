using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class Product
    {
        public string? ProductId { get; set; }
        public string? ItemName { get; set; }
        public decimal? AcqCost { get; set; }
        public decimal? NominatedPrice { get; set; }
        public string? Barcode { get; set; }
        public int? UnitQuantity { get; set; }
        public string? Status { get; set; }
        public int? ReorderPoint { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
