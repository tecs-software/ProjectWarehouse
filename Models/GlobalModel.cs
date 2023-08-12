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
        #region sender credentials
        public static int sender_id { get; set; }
        public static string sender_name { get; set; } = string.Empty;
        public static string sender_province { get; set; } = string.Empty;
        public static string sender_city { get; set; } = string.Empty;
        public static string sender_area { get; set; } = string.Empty;
        public static string sender_phone { get; set; } = string.Empty;
        public static string sender_address { get; set; } = string.Empty;

        public static string key { get; set; }= string.Empty;
        public static string eccompany_id { get; set; } = string.Empty;
        public static string customer_id { get; set; } = string.Empty;
        #endregion

        #region order pick up
        public static string session_id { get; set; } = string.Empty;
        #endregion
        #region get version
        public static string version { get; set; } = string.Empty;
        #endregion
    }
}
