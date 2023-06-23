﻿using System;
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

namespace WarehouseManagement.Controller
{
    public class Create_api
    {
        sql_control sql = new sql_control();
        db_queries queries = new db_queries();
        public void api_create(Customer customer, Receiver receiver, Booking_info booking_Info)
        {
            
            string url = "https://test-api.jtexpress.ph/jts-phl-order-api/api/order/create";
            string eccompanyid = "THIRDYNAL";
            string key = "8049bdb499fc06b6fde3e476a87987ef";
            string logistics_interface = @"
            {
            ""actiontype"": ""add"",
            ""environment"": ""staging:yes"",
            ""eccompanyid"": ""THIRDYNAL"",
            ""customerid"": ""CS-V0234"",
            ""txlogisticid"": ""1547191-2707123"",
            ""ordertype"": ""1"",
            ""servicetype"": ""6"",
            ""deliverytype"": ""1"",
            ""sender"": {
                ""name"": ""grace"",
                ""mobile"": ""09951455616"",
                ""prov"": ""METRO-MANILA"",
                ""city"": ""TAGUIG"",
                ""area"": ""BAGUMBAYAN"",
                ""address"": ""#20 1st AVE. STA. MARIA INDUSTRIAL BAGUMBAYAN TAGUIG CITY""
            },
            ""receiver"": {
                ""name"": ""Kathea de guzman"",
                ""mobile"": ""0917 536 7871"",
                ""phone"": """",
                ""prov"": ""METRO-MANILA"",
                ""city"": ""QUEZON-CITY"",
                ""area"": ""BAGONG LIPUNAN"",
                ""address"": ""#20 1st AVE. STA. MARIA INDUSTRIAL BAGUMBAYAN TAGUIG CITY""
            },
            ""createordertime"": ""2020-07-09 22:48:26"",
            ""paytype"": ""1"",
            ""weight"": ""0.1"",
            ""itemsvalue"": ""1599"",
            ""totalquantity"": ""1"",
            ""remark"": """",
            ""items"": [
                {
                    ""itemname"": ""Strap wide leg pants"",
                    ""number"": ""1"",
                    ""itemvalue"": ""1599.000"",
                    ""desc"": ""Strap wide leg pants""
                }
            ]
            }";
            string msg_type = "ORDERCREATE";

            dynamic payloadObj = Newtonsoft.Json.JsonConvert.DeserializeObject(logistics_interface);

            //updating sender information
            payloadObj.sender.name = customer.FirstName + " " + customer.MiddleName + " " + customer.LastName;
            payloadObj.sender.phone = customer.Phone;
            payloadObj.sender.mobile = customer.Phone;
            payloadObj.sender.prov = customer.Province;
            payloadObj.sender.city = customer.City;
            payloadObj.sender.area = customer.Barangay;
            payloadObj.sender.address = customer.Address;

            //updating receiver information
            payloadObj.receiver.name = receiver.FirstName + " " + receiver.MiddleName + " " + receiver.LastName;
            payloadObj.receiver.phone = receiver.Phone;
            payloadObj.receiver.mobile = receiver.Phone;
            payloadObj.receiver.prov = receiver.Province;
            payloadObj.receiver.city = receiver.City;
            payloadObj.receiver.area = receiver.Barangay;
            payloadObj.receiver.address = receiver.Address;
            
            //updating other fields
            payloadObj.createordertime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            payloadObj.weight = booking_Info.weight;
            payloadObj.itemsvalue = booking_Info.goods_value;
            payloadObj.totalquantity = booking_Info.quantity;
            payloadObj.remark = booking_Info.remarks;

            //updating items field
            var itemsArray = payloadObj["items"] as JArray; // Assuming payloadObj is your JSON object
            if (itemsArray != null && itemsArray.Count > 0)
            {
                var firstItem = itemsArray[0];
                firstItem["itemname"] = booking_Info.item_name;
                firstItem["number"] = booking_Info.quantity;
                firstItem["itemvalue"] = booking_Info.goods_value;
            }

            sql.Query($"SELECT order_id FROM tbl_orders ORDER BY order_id DESC");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                string txlogisticid = sql.ReturnResult($"SELECT order_id FROM tbl_orders ORDER BY order_id DESC");
                string[] parts = txlogisticid.Split('-');
                if (parts.Length == 2)
                {
                    string prefix = parts[0];
                    int numericPortion;

                    if (int.TryParse(parts[1], out numericPortion))
                    {
                        // Increment the numeric portion
                        numericPortion++;

                        // Concatenate the prefix and the incremented numeric portion
                        string newTxLogisticId = prefix + "-" + numericPortion.ToString();

                        payloadObj.txlogisticid = newTxLogisticId;
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    
                }
            }
            else
            {
                payloadObj.txlogisticid = "ABC-" + new Random().Next(100000, 999999).ToString();
            }

            string updatedPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payloadObj);
            //MessageBox.Show(updatedPayload);
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
                    requestData.Add("msg_type", msg_type);
                    requestData.Add("eccompanyid", eccompanyid);

                    // Send the POST request
                    byte[] responseBytes = client.UploadValues(url, requestData);

                    // Decode and display the response
                    string response = Encoding.UTF8.GetString(responseBytes);

                    //to decode the response
                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                    string logisticProviderId = responseObject.logisticproviderid;
                    dynamic responseItems = responseObject.responseitems[0];
                    string success = responseItems.success;
                    string reason = responseItems.reason;
                    string txLogisticId = responseItems.txlogisticid;
                    string mailNo = responseItems.mailno;
                    string sortingCode = responseItems.sortingcode;

                    // Store the parameters in separate strings
                    string logisticProviderIdString = logisticProviderId.ToString();
                    string successString = success.ToString();
                    string reasonString = reason.ToString();
                    
                    
                    //if there is no error
                    if (successString == "true")
                    {
                        string txLogisticIdString = txLogisticId.ToString();
                        string mailNoString = mailNo.ToString();
                        string sortingCodeString = sortingCode.ToString();


                        queries.insert_sender(customer);
                        queries.insert_receiver(receiver);

                        queries.Insert_Orders(txLogisticIdString, mailNoString, booking_Info);

                        queries.insert_Incentives(booking_Info);
                        MessageBox.Show("Order has been Created");
                    }
                    //if there's error on API
                    else
                    {
                        MessageBox.Show(response);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        public static class MD5Util
        {
            public static string GetMD5Hash(string input)
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        builder.Append(hashBytes[i].ToString("x2"));
                    }

                    return builder.ToString();
                }
            }
        }
    }
}
