using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class WaybillController
    {
        static sql_control sql = new sql_control();
        public static async Task Insert(
            string Order_id,
            string Waybill,
            string SortingCode,
            string SortingNo,
            string receiverName,
            string receiverProvince,
            string receiverCity,
            string receiverBarangay,
            string receiverAddress,
            string senderName,
            string senderAddress,               
            decimal cod,
            string goods,
            decimal price,
            decimal weight,
            string remarks
            )
        {
            await Task.Run(() => sql.Query($"INSERT INTO tbl_waybill (Order_ID, Waybill, Sorting_Code, Sorting_No, ReceiverName,ReceiverProvince, ReceiverCity, ReceiverBarangay,ReceiverAddress,SenderName,SenderAddress,COD, Goods, Price, Weight, Remarks) " +
                $"VALUES ('{Order_id}', '{Waybill}', '{SortingCode}', '{SortingNo}', '{receiverName}', '{receiverProvince}', '{receiverCity}', '{receiverBarangay}', '{receiverAddress}',  " +
                $" '{senderName}','{senderAddress}', {cod}, '{goods}', {price},{weight}, '{remarks}') "));
            if (sql.HasException(true)) return;
        }
    }
}
