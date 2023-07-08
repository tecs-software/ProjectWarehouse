using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class ExpensesController
    {
        static sql_control sql = new sql_control();

        public static void InsertExpenses(Expenses model) => sql.Query($"EXEC SpExpenses_Insert {model.UserID}, {model.AdSpent},{model.Utilities},{ model.Miscellaneous} ");
    }
}
