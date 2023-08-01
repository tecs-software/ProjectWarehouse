using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WarehouseManagement.Controller;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Models
{
    public class SystemSettingsModel
    {
        // receivers
        public string? receiver_name { get; set; }
        public string? receiver_phone { get; set; }
        public string? receiver_address { get; set; }

        // other fields
        public string? remarks { get; set; }
        public string? product_name { get; set; } //items
        public int? quantity { get; set; }
        public decimal? weight { get; set; }

        //etc
        public decimal? parcel_value { get; set; }
        public decimal? cod { get; set; }
        public string? parcel_name { get; set; }

        //id's
        public string? waybill { get; set; }
        public string? order_id { get; set; }
        public string? sender_name { get; set; }

        //date
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
