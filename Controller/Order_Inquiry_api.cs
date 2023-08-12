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
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Diagnostics.Eventing.Reader;
using static WarehouseManagement.Controller.Show_order_inquiry;
using static WarehouseManagement.Views.Main.DeliverModule.DeliveryTable;
using System.Data;
using WarehouseManagement.Views.Main.DeliverModule;

namespace WarehouseManagement.Controller
{
    public class Order_Inquiry_api
    {
        static sql_control sql = new sql_control();
        public async Task insert_inquirt(string waybill, TextBox txtReceiverName, TextBox txtContactNumber, TextBox txtAddress, TextBox txtProvince, TextBox txtCity, TextBox txtBarangay, TextBox txtDateCreated, TextBox txtRemarks, TextBox txtWeight, TextBox txtQuantity, TextBox txtProductName, TextBox date, string session)
        {
            int inquiry_count = int.Parse(sql.ReturnResult($"SELECT COUNT(waybill#) FROM tbl_order_inquiry WHERE waybill# = '{waybill}' AND session_id = '{session}'"));
            if(inquiry_count > 0 )
            {
                
            }
            else
            {
                string address = txtProvince.Text + "/" + txtCity.Text + "/" + txtBarangay.Text;
                sql.AddParam("@waybill", waybill);
                sql.AddParam("@name", txtReceiverName.Text);
                sql.AddParam("@contact", txtContactNumber.Text);
                sql.AddParam("@address", address);
                sql.AddParam("@product", txtProductName.Text);
                sql.AddParam("@qty", txtQuantity.Text);
                sql.AddParam("@weight", txtWeight.Text);
                sql.AddParam("@remarks", txtRemarks.Text);
                sql.AddParam("@date", txtDateCreated.Text);
                sql.AddParam("@session", session);

                sql.Query($"INSERT INTO tbl_order_inquiry (waybill#, receiver_name, contact_number, address, product_name, qty, weight, remarks, date_created, session_id) " +
                    $"VALUES (@waybill, @name, @contact, @address, @product, @qty, @weight, @remarks, @date, @session)");
                if (sql.HasException(true)) return;
            }
        }
        public async Task inquiry_api(string waybill, TextBox txtReceiverName, TextBox txtContactNumber, TextBox txtAddress, TextBox txtProvince, TextBox txtCity, TextBox txtBarangay, TextBox txtDateCreated, TextBox txtRemarks, TextBox txtWeight, TextBox txtQuantity, TextBox txtProductName)
        {
            string url = "https://jtapi.jtexpress.ph/jts-phl-order-api/api/order/queryOrder";
            string eccompanyid = sql.ReturnResult($"SELECT eccompany_id FROM tbl_couriers");
            string key = Decrypt(sql.ReturnResult($"SELECT api_key FROM tbl_couriers"));
            string logistics_interface = @"
            {
                ""eccompanyid"": ""THIRDYNAL"",
                ""customerid"": ""NWL-F3911"",
                ""command"": ""2"",
                ""serialnumber"": """",
                ""startdate"": """",
                ""enddate"": """"  
            }";
            dynamic payloadObj = Newtonsoft.Json.JsonConvert.DeserializeObject(logistics_interface);
            payloadObj.serialnumber = waybill;
            payloadObj.startdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            payloadObj.enddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            payloadObj.eccompanyid = eccompanyid;
            payloadObj.customerid = sql.ReturnResult($"SELECT customer_id FROM tbl_couriers");

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
                    requestData.Add("msg_type", "ORDERQUERY");
                    requestData.Add("eccompanyid", eccompanyid);

                    // Send the POST request
                    byte[] responseBytes = await client.UploadValuesTaskAsync(url, requestData);

                    // Decode and display the response
                    string response = Encoding.UTF8.GetString(responseBytes);
                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                    string logisticProviderId = responseObject.logisticproviderid;
                    dynamic responseItems = responseObject.responseitems;
                    dynamic orderList = responseItems[0].orderList;

                    if(orderList == null)
                    {
                        //do nothing
                    }
                    else
                    {
                        foreach (var order in orderList)
                        {
                            dynamic receiver = order.receiver;
                            dynamic items = order.items;

                            //receivers response
                            string receiverName = receiver.name;
                            string receiverPhone = receiver.phone;
                            string receiverMobile = receiver.mobile;
                            string receiverProvince = receiver.prov;
                            string receiverCity = receiver.city;
                            string receiverArea = receiver.area;
                            string receiverAddress = receiver.address;

                            txtReceiverName.Text = receiverName;
                            txtContactNumber.Text = receiverPhone;
                            txtAddress.Text = receiverAddress;
                            txtProvince.Text = receiverProvince;
                            txtCity.Text = receiverCity;
                            txtBarangay.Text = receiverArea;

                            string order_created = order.createordertime;
                            string weight = order.weight;
                            string remarks = order.remark;
                            string quantity = order.totalquantity;
                            txtDateCreated.Text = DateTime.Parse(order_created).ToString("yyyy-MMM-dd HH:mm:ss");
                            txtRemarks.Text = remarks;
                            txtWeight.Text = weight;
                            txtQuantity.Text = quantity;

                            string goodsNames = order.goodsNames;
                            txtProductName.Text = goodsNames;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        public static string setSession_id()
        {
            string session_id = sql.ReturnResult($"SELECT TOP 1(session_id) FROM tbl_order_inquiry ORDER BY order_inquiry_id DESC");
            return session_id;
        }
        public static void InsertSessions(ComboBox cmbsessions)
        {
            sql.Query($"SELECT session_id FROM tbl_order_inquiry GROUP BY session_id");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    cmbsessions.Items.Add(dr[0].ToString());
                }
            }
        }
        public static string setParcelCount(string session_id)
        {
            string count = sql.ReturnResult($"SELECT count(order_inquiry_id) FROM tbl_order_inquiry WHERE session_id = '{session_id}'");
            return count;
        }
        public static string GenerateSession_id()
        {
            var rd = new Random();
            string session_id = DateTime.Now.ToString("MM/dd/yyyy") + "-" + rd.Next(1000).ToString();
            sql.Query($"SELECT count(order_inquiry_id) FROM tbl_order_inquiry WHERE session_id = '{session_id}'");
            if(sql.DBDT.Rows.Count > 0)
            {
                return session_id;
            }
            else
            {
                return GenerateSession_id();
            }
        }
    }
}
