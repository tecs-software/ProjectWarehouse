using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class UserController
    {
        static sql_control sql = new sql_control();
        public static void LoadSender(ComboBox cmb)
        {
            cmb.Items.Clear();
            sql.Query("SELECT sender_name FROM tbl_sender ");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    cmb.Items.Add(dr[0].ToString());
                }
            }
        }

        public static void UpdateSender(string? id,string senderName) => sql.Query($"UPDATE tbl_users SET sender_id = {GetSenderID(senderName)} WHERE user_id = {int.Parse(id)} ");
        public static int GetSenderID(string senderName) => int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_sender WHERE sender_name = '{senderName}' "));
        public static string GetSenderName(string? user_id) 
            => sql.ReturnResult($"SELECT sender_name FROM tbl_users LEFT JOIN tbl_sender ON tbl_users.sender_id = tbl_sender.sender_id WHERE user_id = {int.Parse(user_id)} ");

    }
}
