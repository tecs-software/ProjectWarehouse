using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;
using System.Data;
using Newtonsoft.Json.Linq;
using WarehouseManagement.Database;
using WWarehouseManagement.Database;
using System.IO;
using System.Security.Cryptography;
using WarehouseManagement.Views.Main.ShopModule;
using WarehouseManagement.Views.Main.DeliverModule;

namespace WarehouseManagement.Controller
{
    public class ShopController
    {
        sql_control sql = new sql_control();
        public static bool exceedResult { get; set; } = false;

        public void populate_shops(ComboBox shop_list)
        {
            sql.Query($"SELECT sender_name FROM tbl_sender");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    shop_list.Items.Add(dr[0].ToString());
                }
            }
        }
        public void display_shop_data(DataGrid dgt_shops, ComboBox shop_list, bool clickedNext)
        {
            int result_count;
            if (clickedNext)
            {
                if (!exceedResult)
                {
                    ShopView.offsetCount = ShopView.offsetCount + 12;
                }
            }
            else
            {
                if (ShopView.offsetCount == 0)
                    ShopView.offsetCount = 0;
                else
                {
                    ShopView.offsetCount = ShopView.offsetCount - 12;
                    exceedResult = false;
                }

            }

            if (shop_list.SelectedIndex == 0)
            {
                sql.Query($"SELECT * FROM tbl_orders ORDER BY created_at DESC OFFSET {ShopView.offsetCount} ROWS FETCH NEXT 12 ROWS ONLY;");
                result_count = sql.DBDT.Rows.Count;

                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    List<shopData> shops = new List<shopData>();
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        shopData shop_data = new shopData
                        {
                            name = sql.ReturnResult($"SELECT first_name FROM tbl_users WHERE user_id = {int.Parse(dr[3].ToString())}"),
                            shop_name = sql.ReturnResult($"SELECT sender_name FROM tbl_sender WHERE sender_id = {int.Parse(dr[4].ToString())}"),
                            waybill = dr[2].ToString(),
                            courier = dr[1].ToString(),
                            product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '{dr[6].ToString()}'"),
                            //receiver = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[5].ToString())}"),
                            total_price = decimal.Parse(dr[8].ToString()),
                            status = dr[10].ToString(),
                            last_update = dr[12].ToString()
                        };
                        shops.Add(shop_data);
                    }
                    dgt_shops.ItemsSource = shops;

                    exceedResult = false;

                    if (clickedNext)
                    {
                        if (result_count < 11)
                        {
                            exceedResult = true;
                            return;
                        }

                    }
                }
                else
                {
                    dgt_shops.ItemsSource = null;
                }
            }
            else
            {
                sql.AddParam("@shop_list", shop_list.Text);
                int? shop_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_sender WHERE sender_name = @shop_list"));

                sql.Query($"SELECT * FROM tbl_orders WHERE sender_id = {shop_id}  ORDER BY created_at DESC OFFSET {ShopView.offsetCount} ROWS FETCH NEXT 12 ROWS ONLY;");
                result_count = sql.DBDT.Rows.Count;

                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    List<shopData> shops = new List<shopData>();
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        shopData shop_data = new shopData
                        {
                            name = sql.ReturnResult($"SELECT first_name FROM tbl_users WHERE user_id = {int.Parse(dr[3].ToString())}"),
                            shop_name = sql.ReturnResult($"SELECT sender_name FROM tbl_sender WHERE sender_id = {int.Parse(dr[4].ToString())}"),
                            waybill = dr[2].ToString(),
                            courier = dr[1].ToString(),
                            product = sql.ReturnResult($"SELECT item_name FROM tbl_products WHERE product_id = '{dr[6].ToString()}'"),
                            //receiver = sql.ReturnResult($"SELECT receiver_name FROM tbl_receiver WHERE receiver_id = {int.Parse(dr[5].ToString())}"),
                            total_price = decimal.Parse(dr[8].ToString()),
                            status = dr[10].ToString(),
                            last_update = dr[12].ToString()
                        };
                        shops.Add(shop_data);
                    }
                    dgt_shops.ItemsSource = shops;

                    exceedResult = false;

                    if (clickedNext)
                    {
                        if (result_count < 11)
                        {
                            exceedResult = true;
                            return;
                        }
                    }
                }
                else
                {
                    dgt_shops.ItemsSource = null;
                }
            }
        }
        public class shopData
        {
            public string name { get; set; }
            public string? shop_name { get; set; }
            public string? waybill { get; set; }
            public string? courier { get; set; }
            public string? product { get; set; }
            public string? receiver { get; set; }
            public decimal? total_price { get; set; }
            public string? status { get; set; }
            public string? last_update { get; set; }
        }
    }
}
