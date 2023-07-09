using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;
using System.Windows;

namespace WarehouseManagement.Controller
{
    public class ExpensesController
    {
        static sql_control sql = new sql_control();

        public static void InsertExpenses(Expenses model) => sql.Query($"EXEC SpExpenses_Insert {model.UserID}, {model.AdSpent},{model.Utilities},{ model.Miscellaneous} ");

        public async static Task showTotalExpenses(System.Windows.Controls.Label total_expenses, int days)
        {
            await Task.Run(() =>
            {
                sql.Query($"EXEC SpExpenses_GetDataFilterByDate '{DateTime.Now.AddDays(1)}', {days}");
                if (sql.HasException(true)) return;

                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        total_expenses.Dispatcher.Invoke(() =>
                        {
                            total_expenses.Content = dr[5].ToString();
                        });
                    }
                }
            });
        }
    }
}
