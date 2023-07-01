using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WWarehouseManagement.Database;
using static WarehouseManagement.Controller.Create_api;
using System.Text.RegularExpressions;

namespace WarehouseManagement.Controller
{
    class Track_api
    {
        sql_control sql = new sql_control();

        public async Task api_track(string waybill, string courier)
        {
            string url = "https://jtapi.jtexpress.ph/jts-phl-order-api/api/track/trackForJson";
            string eccompanyid = sql.ReturnResult($"SELECT eccompany_id FROM tbl_couriers WHERE courier_name = '{courier}'");
            string key = sql.ReturnResult($"SELECT api_key FROM tbl_couriers WHERE courier_name = '{courier}'");
            string logistics_interface = @"
            {
                ""billcode"": """",
                ""lang"": ""en"",
            }";

            dynamic payloadObj = Newtonsoft.Json.JsonConvert.DeserializeObject(logistics_interface);
            payloadObj.billcode = waybill;

            string updatedPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payloadObj);

            try
            {
                // Step 3: Sign the JSON content and secret key
                string data_digest = MD5Util.GetMD5Hash(updatedPayload + key);
                string encodedDataDigest = Convert.ToBase64String(Encoding.UTF8.GetBytes(data_digest));


                // Step 4: Send HTTP POST request
                using (WebClient client = new WebClient())
                {
                    // Prepare request parameters
                    NameValueCollection requestData = new NameValueCollection();
                    requestData.Add("logistics_interface", updatedPayload);
                    requestData.Add("data_digest", encodedDataDigest);
                    requestData.Add("msg_type", "TRACKQUERY");
                    requestData.Add("eccompanyid", eccompanyid);

                    // Send the POST request
                    byte[] responseBytes = await client.UploadValuesTaskAsync(url, requestData);

                    // Decode and display the response
                    string response = Encoding.UTF8.GetString(responseBytes);
                    //MessageBox.Show(response);
                    //Console.WriteLine(response);
                    dynamic responseObject = JsonConvert.DeserializeObject(response);
                    string logisticProviderId = responseObject.logisticproviderid;

                    dynamic responseItem = responseObject.responseitems[0];
                    string reason = responseItem.reason;

                    dynamic[] details = responseItem.details.ToObject<dynamic[]>();

                    foreach (dynamic detail in details)
                    {
                        string scanTime = detail.scantime;
                        string scanType = detail.scantype;
                        string description = detail.desc;

                        sql.Query($"SELECT * FROM tbl_status WHERE scan_time = '{scanTime}' AND waybill# = '{waybill}'");
                        if (sql.HasException(true)) return;
                        if(sql.DBDT.Rows.Count > 0)
                        {
                            foreach(DataRow dr in sql.DBDT.Rows)
                            {
                                string parse_time = DateTime.Parse(dr[4].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                if (parse_time == scanTime)
                                {
                                    
                                }
                                else
                                {
                                    sql.Query($"INSERT INTO tbl_status (waybill#, scan_type, description, scan_time) VALUES ('{waybill}', '" + scanType.Replace('【', ' ') + "', '" + description.Replace('】', ' ') + "', '" + scanTime + "')");
                                    if (sql.HasException(true)) return;
                                }
                            }
                        }
                        else
                        {
                            sql.Query($"INSERT INTO tbl_status (waybill#, scan_type, description, scan_time) VALUES ('{waybill}', '" + scanType.Replace('【', ' ') + "', '" + description.Replace('】', ' ') + "', '" + scanTime + "')");
                            if (sql.HasException(true)) return;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("This parcel hasn't been processed by J&T Express.");
            }
        }
        public async Task update_status(DataGrid dataGrid)
        {
            sql.Query($"SELECT * FROM tbl_status ORDER BY scan_time DESC");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                List<Updates> updates = new List<Updates>();
                
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    Updates update = new Updates
                    {
                        scan_type = dr[2].ToString(),
                        scan_time = DateTime.Parse(dr[4].ToString()).ToString("yyyy-MM-dd hh:mm tt")
                    };
                    updates.Add(update);
                }
                dataGrid.ItemsSource = updates;
            }

        }
        public class Updates
        {
            public string scan_type { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;    
            public string scan_time { get; set; } = string.Empty;
        }
    }
    
}
