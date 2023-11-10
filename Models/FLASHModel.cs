using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class FLASHModel
    {
        #region receiver details
        public string receiver_name { get; set; } = string.Empty;
        public string receiver_province { get; set; } = string.Empty;
        public string receiver_city { get; set; } = string.Empty;
        public string receiver_barangay { get; set; } = string.Empty;
        public string receiver_address { get; set; } = string.Empty;
        public string postal_code { get; set; } = string.Empty;
        public string receiver_phone { get; set; } = string.Empty;
        #endregion

        #region delivery data
        public string express_category { get; set; } = string.Empty;
        public string article_category { get; set; } = string.Empty;
        public string isCOD { get; set; } = string.Empty;
        public string COD { get; set; } = string.Empty;
        #endregion

        #region parcel data
        public string item { get; set; } = string.Empty;
        public string weight { get; set; } = string.Empty;
        public string height { get; set; } = string.Empty;
        public string lenght { get; set; } = string.Empty;
        public string width { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;

        #endregion


    }
    public class FLASHApiResponse<T>
    {
        public string code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public T data { get; set; }
    }
    public class OrderResponse
    {
        public string pno { get; set; } = string.Empty;
        public string outTradeNo { get; set; } = string.Empty;
    }
    public class waybill
    {
        public static string pno { get; set; } = string.Empty;
    }
    public class Route
    {
        public long routedAt { get; set; }
        public string routedAction { get; set; }
        public string message { get; set; }
        public int state { get; set; }
    }
    public class RouteResponse
    {
        public string pno { get; set; }
        public int state { get; set; }
        public string stateText { get; set; }
        public string stateChangeAt { get; set; }
        public List<Route> routes { get; set; }
    }
}
