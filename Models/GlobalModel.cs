using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Helpers;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Models
{
    public class GlobalModel
    {
        sql_control sql = new sql_control();

        public string sender_name { get; set; } = string.Empty;
        public string sender_province { get; set; } = string.Empty;
        public string sender_city { get; set; } = string.Empty;
        public string sender_area { get; set; } = string.Empty;
        public string sender_phone { get; set; } = string.Empty;
        public string sender_address { get; set; } = string.Empty;

    }
}
