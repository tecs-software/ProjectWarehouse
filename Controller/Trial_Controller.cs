using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class Trial_Controller
    {
        static sql_control sql = new sql_control();
        public static bool IsTrialEnded()
        {
            int days = int.Parse(sql.ReturnResult("EXEC Sp_Trial_Validation"));
            if (days <= 1)
                return false;
            else
                return false;
        }
        public static int HaveTrialKey() => int.Parse(sql.ReturnResult("EXEC SpTrial_HaveKey"));
        public static void MessagePopup()
        {
            if (HaveTrialKey() == 0)
            {
                if (Trial_Controller.IsTrialEnded())
                    MessageBox.Show("Trial is expired. Please contact your distributor of the application.");
            }
        }
    }
}
