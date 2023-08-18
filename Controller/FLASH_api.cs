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

namespace WarehouseManagement.Controller
{
    public class FLASH_api
    {
        static sql_control sql = new sql_control();
        public static SortedDictionary<string, string> MockCreateOrderData()
        {
            var rd = new Random();
            var dic = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                {"mchId", GlobalModel.customer_id},
                {"nonceStr",  DateTime.Now.ToString("yyyyMMddHHmmss") + rd.Next(1,10000)},                                        //change on your demand
                {"outTradeNo",  "TECS-" + GenerateTransactionID()},    //order id
                {"expressCategory", FLASHModel.express_category},
                {"srcName", GlobalModel.sender_name},
                {"srcPhone", GlobalModel.sender_phone},
                {"srcProvinceName", GlobalModel.sender_province},
                {"srcCityName", GlobalModel.sender_city},
                {"srcPostalCode",GlobalModel.sender_postal},
                {"srcDetailAddress", GlobalModel.sender_address},
                {"dstName",FLASHModel.receiver_name},
                {"dstPhone", FLASHModel.receiver_phone},
                {"dstProvinceName", FLASHModel.receiver_province},
                {"dstCityName", FLASHModel.receiver_city},
                {"dstPostalCode", FLASHModel.postal_code},
                {"dstDetailAddress", FLASHModel.receiver_address},
                {"articleCategory", FLASHModel.article_category},
                {"weight", FLASHModel.weight},
                {"codEnabled",FLASHModel.isCOD},
                {"codAmount",FLASHModel.COD},
                {"remark", FLASHModel.remarks},
                {"width",FLASHModel.width},
                {"height",FLASHModel.height},
                {"length",FLASHModel.lenght},
            };
            return dic;
        }
        public static async Task FlashCreateOrder()
        {
            var mockData = MockCreateOrderData();
            var url = "/open/v1/orders";
            var responseData = await RequestDataAsync<OrderResponse>(url, mockData, GlobalModel.customer_id);
            if (responseData.code == "1")
            {
                MessageBox.Show($"Create Order successfully! The pno={responseData.data.pno}{Environment.NewLine}");
                MessageBox.Show($"Create Order successfully! The Order ID={responseData.data.outTradeNo}{Environment.NewLine}");
                waybill.pno = responseData.data.pno;
            }
            else
            {
                MessageBox.Show($"Get warehousers failed! The error message ={responseData}{Environment.NewLine}");
            }
        }
        public static long GenerateTransactionID()
        {
            var finalString = "";
            var chars = "1234567";
            var stringChars = new char[9];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            finalString = new String(stringChars);

            sql.Query($"SELECT * FROM tbl_orders WHERE order_id = '{"TECS-" + finalString}'");
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
                    var response = await httpClient.PostAsync("https://open-api-tra.flashexpress.ph" + url, content);

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
