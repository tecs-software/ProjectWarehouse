﻿using System;
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
        public static void InsertTrialDay() {
            sql.Query("EXEC Sp_Trial_Insertion");
            MessageBox.Show("Test");
        } 
        public static int HaveTrialKey() => int.Parse(sql.ReturnResult("EXEC SpTrial_HaveKey"));
        public static void MessagePopup()
        {
            //if (Trial_Controller.IsTrialEnded())
            //    MessageBox.Show("Trial is expired. Please contact your distributor of the application.");
        }
        public static bool IsTrialEnded()
        {
            int days = int.Parse(sql.ReturnResult("EXEC Sp_Trial_Validation"));
            if (days >= 7)
                return true;
            else
                return false;
        }
        public static void updateModules()
        {
            int? count = int.Parse(sql.ReturnResult($"SELECT COUNT(*) from tbl_module_access WHERE module_name = 'modify order inquiry'"));
            if(count > 0)
            {
                sql.Query($"UPDATE tbl_module_access SET module_name = 'Modify Out For Pick Up' WHERE module_name = 'modify order inquiry'");
                sql.Query($"UPDATE tbl_module_access SET module_name = 'View Out For Pick Up' WHERE module_name = 'view order inquiry'");
            }
            else
            {
                //do nothing
            }

        }
    }
}
