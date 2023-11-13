using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Mime;
using System.Web;
using Newtonsoft.Json;
using System.Security.Cryptography;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;
using System.Windows.Input;
using System.Windows;
using WarehouseManagement.Database;
using System.Windows.Controls;

namespace WarehouseManagement.Controller
{
    public class FLASH_api
    {
        static sql_control sql = new sql_control();
        db_queries queries = new db_queries();
        public static SortedDictionary<string, string> MockCreateOrderData(FLASHModel flashmodel)
        {
            decimal codAmount = decimal.Parse(flashmodel.COD) * 100;
            var rd = new Random();
            var dic = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                {"mchId", GlobalModel.customer_id},
                {"nonceStr",  DateTime.Now.ToString("yyyyMMddHHmmss") + rd.Next(1,10000)},//change on your demand
                {"outTradeNo",  "TECS-F" + GenerateTransactionID()},    //order id
                {"expressCategory", "1"},
                {"srcName", GlobalModel.sender_name},
                {"srcPhone", GlobalModel.sender_phone},
                {"srcProvinceName", GlobalModel.sender_province},
                {"srcCityName", GlobalModel.sender_city},
                {"srcPostalCode",GlobalModel.sender_postal},
                {"srcDetailAddress", GlobalModel.sender_address},
                {"dstName", flashmodel.receiver_name},
                {"dstPhone", flashmodel.receiver_phone},
                {"dstProvinceName", flashmodel.receiver_province},
                {"dstCityName", flashmodel.receiver_city},
                {"dstPostalCode", flashmodel.postal_code},
                {"dstDetailAddress", flashmodel.receiver_address},
                {"articleCategory", flashmodel.article_category},
                {"weight", flashmodel.weight},
                {"codEnabled",flashmodel.isCOD},
                {"codAmount",codAmount.ToString()},
                {"remark", flashmodel.remarks},
                {"width",flashmodel.width},
                {"height",flashmodel.height},
                {"length",flashmodel.lenght},
            };
            return dic;
        }
        public static SortedDictionary<string, string> MockCommonData(string dateString = "")
        {
            var rd = new Random();
            var dic = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                {"mchId", GlobalModel.customer_id},
                {"nonceStr", DateTime.Now.ToString("yyyyMMddHHmmss") + rd.Next(1,10000)}, //change on your demand
            };
            return dic;
        }
        public static SortedDictionary<string, string> CreateSubAccountData(FlashAccountDetails details)
        {
            var rd = new Random();
            var dic = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                {"mchId", GlobalModel.customer_id},
                {"nonceStr", DateTime.Now.ToString("yyyyMMddHHmmss") + rd.Next(1,10000)}, //change on your demand
                {"accountName",details.AccountName},
                {"name",details.Fullname},
                {"mobile",details.Mobile},
                {"email",details.Email},
                {"showAmountEnabled","1"},
            };
            return dic;
        }
        public static async Task<bool> FlashCreateSubaccount(TextBox Subaccountid, TextBox Accountname, TextBox name, FlashAccountDetails details)
        {
            var mockData = CreateSubAccountData(details);
            var url = "/open/v1/new_sub_account";
            var responseData = await RequestDataAsync<AccountResponse>(url, mockData, GlobalModel.customer_id);
            if (responseData.code == "1")
            {
                Subaccountid.Text = responseData.data.Subaccountid;
                Accountname.Text = responseData.data.AccountName;
                name.Text = responseData.data.Name;
                return true;
            }
            else
            {
                MessageBox.Show($"Order process failed! The error message ={responseData}{Environment.NewLine}");
                return false;
            }
        }
        public static async Task<bool> FlashCreateOrder(FLASHModel model)
        {
            var mockData = MockCreateOrderData(model);
            var url = "/open/v1/orders";
            var responseData = await RequestDataAsync<OrderResponse>(url, mockData, GlobalModel.customer_id);
            if (responseData.code == "1")
            {
                FlashDB.ReceiverData(model);
                FlashDB.OrderData(model, responseData.data.outTradeNo, responseData.data.pno);
                FlashDB.UpdateStocks(model);
                waybill.pno = responseData.data.pno;
                return true;
            }
            else
            {
                MessageBox.Show($"Order process failed! The error message ={responseData}{Environment.NewLine}");
                return false;
            }
        }
        public static async Task<bool> FlashCancelOrder(string pno, string reason, string product, string id)
        {
            var mockData = MockCommonData();
            var url = $"/open/v1/orders/{pno}/cancel";
            var responseData = await RequestDataAsync<OrderResponse>(url, mockData, GlobalModel.customer_id);
            if (responseData.code == "1")
            {
                FlashDB.UpdateCancelledOrder(reason, product, id);
                MessageBox.Show("Order cancelled");
                return true;
            }
            else
            {
                MessageBox.Show($"Get warehousers failed! The error message ={responseData}{Environment.NewLine}");
                return false;
            }
        }
        public static long GenerateTransactionID()
        {
            var finalString = "";
            var chars = "1234567";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            finalString = new String(stringChars);

            sql.Query($"SELECT * FROM tbl_orders WHERE order_id = '{"TECS-F" + finalString}'");
            if (sql.DBDT.Rows.Count == 0)
            {
                return long.Parse(finalString);
            }
            else
            {
                return GenerateTransactionID();
            }
        }
        public static string Sign(string prestr, string _input_charset)
        {
            var sb = new StringBuilder(32);
            var sha256 = new SHA256Managed();
            var t = sha256.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
            foreach (var t1 in t)
            {
                sb.Append(t1.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }
        public static string BuildRequestContent(string key, SortedDictionary<string, string> dicParam)
        {
            //get sign
            var prestr = new StringBuilder();
            var urlencodeSB = new StringBuilder();
            foreach (var temp in dicParam.OrderBy(o => o.Key, StringComparer.Ordinal))
            {
                urlencodeSB.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, System.Text.Encoding.UTF8).Replace("+", "%20") + "&");
                if (temp.Value.Trim() == String.Empty)
                    continue;                                   //empty value not participate in signature.
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }
            prestr.Remove(prestr.Length - 1, 1); //remove the last '&' to form the form basic request content

            var str_to_sign = prestr.ToString() + "&key=" + key;
            var sign = Sign(str_to_sign, "utf-8").ToUpper();

            var urlencode_str = urlencodeSB.ToString();
            var return_str = urlencode_str.Remove(urlencode_str.Length - 1, 1) + "&sign=" + sign;
            return return_str;
        }
        private static async Task<FLASHApiResponse<T>> RequestDataAsync<T>(string url, SortedDictionary<string, string> requestData, string merchantPW, string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                var requestContent = BuildRequestContent(Decrypt(GlobalModel.key), requestData);

                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(requestContent, Encoding.UTF8, contentType);
                    var response = await httpClient.PostAsync("https://open-api.flashexpress.ph" + url, content);

                    response.EnsureSuccessStatusCode(); // Ensure the response is successful (status code 2xx).

                    string responseString = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(responseString);
                    // Deserialize the response into FLASHApiResponse<T>.
                    var responseData = JsonConvert.DeserializeObject<FLASHApiResponse<T>>(responseString);
                    return responseData;
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle specific exception for HTTP request errors.
                throw new Exception("HTTP request error: " + ex.Message, ex);
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization exceptions.
                throw new Exception("JSON deserialization error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions.
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }
        public static string Decrypt(string encryptedText)
        {
            string key = "YourEncryptionKey"; // Replace with your desired encryption key

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(encryptedBytes);
        }
    }
}
