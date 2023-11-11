using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static WarehouseManagement.Controller.Create_api;
using System.Windows;
using WWarehouseManagement.Database;
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Controller
{
    class Cancel_api
    {
        sql_control sql = new sql_control();
        public async Task<bool> api_cancel(string id, string reason, string courier, string product)
        {
            string url = "https://jtapi.jtexpress.ph/jts-phl-order-api/api/order/cancel";
            string eccompanyid = sql.ReturnResult($"SELECT eccompany_id FROM tbl_couriers WHERE courier_name = '{courier}'");
            string key = Decrypt(sql.ReturnResult($"SELECT api_key FROM tbl_couriers WHERE courier_name = '{courier}'"));
            string logistics_interface = @"
            {
                ""eccompanyid"": ""THIRDYNAL"",
                ""customerid"": ""CS-V0234"",
                ""txlogisticid"": ""1537212-2707123"",
                ""reason"": ""Panget""
            }";

            dynamic payloadObj = Newtonsoft.Json.JsonConvert.DeserializeObject(logistics_interface);
            payloadObj.eccompanyid = sql.ReturnResult($"SELECT eccompany_id FROM tbl_couriers WHERE courier_name = '{courier}'");
            payloadObj.customerid = sql.ReturnResult($"SELECT customer_id FROM tbl_couriers WHERE courier_name = '{courier}'");
            payloadObj.txlogisticid = id;
            payloadObj.reason = reason;

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
                    requestData.Add("msg_type", "ORDERCANCEL");
                    requestData.Add("eccompanyid", eccompanyid);

                    // Send the POST request
                    byte[] responseBytes = await client.UploadValuesTaskAsync(url, requestData);

                    // Decode and display the response
                    string response = Encoding.UTF8.GetString(responseBytes);

                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                    string logisticProviderId = responseObject.logisticproviderid;
                    dynamic responseItems = responseObject.responseitems[0];
                    string success = responseItems.success;
                    string failed_reason = responseItems.reason;
                    string txLogisticId = responseItems.txlogisticid;

                    if(success == "true")
                    {
                        sql.Query($"UPDATE tbl_orders SET remarks = '{reason}', status = 'CANCELLED', updated_at = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE order_id = '{id}'");
                        MessageBox.Show("Order has been Cancelled");

                        int order_qty = int.Parse(sql.ReturnResult($"SELECT quantity FROM tbl_orders WHERE order_id = '{id}'"));
                        sql.Query($"UPDATE tbl_products SET unit_quantity = unit_quantity+{order_qty} WHERE item_name = '{product}'");

                        int stocks = int.Parse(sql.ReturnResult($"SELECT unit_quantity FROM tbl_products WHERE item_name = '{product}'"));
                        string status = stocks < 0 ? Util.status_out_of_stock : (stocks == 0 ? Util.status_out_of_stock : (stocks <= 100 ? Util.status_low_stock : Util.status_in_stock));
                        sql.Query($"UPDATE tbl_products SET status = '{status}' WHERE item_name = '{product}'");

                        //invalidating the incentives
                        sql.Query($"UPDATE tbl_incentives SET is_valid = 0 WHERE incentive_for = '{id}'");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Cancellation Error", response);
                        return false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return false;
            }
        }
    }
}
