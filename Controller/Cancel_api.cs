﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static WarehouseManagement.Controller.Create_api;
using System.Windows;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    class Cancel_api
    {
        sql_control sql = new sql_control();
        public void api_cancel(string id, string reason)
        {
            string url = "https://test-api.jtexpress.ph/jts-phl-order-api/api/order/cancel";
            string eccompanyid = "THIRDYNAL";
            string key = "8049bdb499fc06b6fde3e476a87987ef";
            string logistics_interface = @"
            {
                ""eccompanyid"": ""THIRDYNAL"",
                ""customerid"": ""CS-V0234"",
                ""txlogisticid"": ""1537212-2707123"",
                ""reason"": ""Panget""
            }";

            dynamic payloadObj = Newtonsoft.Json.JsonConvert.DeserializeObject(logistics_interface);
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
                    byte[] responseBytes = client.UploadValues(url, requestData);

                    // Decode and display the response
                    string response = Encoding.UTF8.GetString(responseBytes);

                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                    string logisticProviderId = responseObject.logisticproviderid;
                    dynamic responseItems = responseObject.responseitems[0];
                    string success = responseItems.success;
                    string failed_reason = responseItems.reason;
                    string txLogisticId = responseItems.txlogisticid;

                    // Store the parameters in separate strings
                    string logisticProviderIdString = logisticProviderId.ToString();
                    string successString = success.ToString();
                    string reasonString = reason.ToString();
                    string txLogisticIdString = txLogisticId.ToString();

                    if(successString == "true")
                    {
                        sql.Query($"UPDATE tbl_orders SET status = 'CANCELLED', updated_at = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE order_id = '" + id + "'");
                        if (sql.HasException(true)) return;
                        MessageBox.Show("Order has been Cancelled");
                    }
                    else
                    {
                        MessageBox.Show("Cancellation Error", response);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}