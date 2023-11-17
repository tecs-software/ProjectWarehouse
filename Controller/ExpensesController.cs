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
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using LiveCharts;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Diagnostics.Eventing.Reader;

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
                DateTime dateTime = DateTime.Now.AddDays(1).Date;
                sql.AddParam("@date", dateTime);
                sql.Query($"EXEC SpExpenses_GetDataFilterByDate @date, {days}");
                if (sql.HasException(true)) return;

                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        total_expenses.Dispatcher.Invoke(() =>
                        {
                            if (dr[5].ToString() == "" || dr[5].ToString() == null)
                                total_expenses.Content = "0";
                            else
                                total_expenses.Content = dr[5].ToString();
                        });
                    }
                }
            });
        }
        public async static Task showExpensesData(System.Windows.Controls.Label total_expenses, System.Windows.Controls.Label adspent, System.Windows.Controls.Label utilities, System.Windows.Controls.Label misc, int days)
        {
            await Task.Run(() =>
            {
                DateTime dateTime = DateTime.Now.AddDays(1).Date;
                sql.AddParam("@date", dateTime);
                sql.Query($"EXEC SpExpenses_GetDataFilterByDate @date, {days}");
                if (sql.HasException(true)) return;

                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        total_expenses.Dispatcher.Invoke(() =>
                        {
                            if (dr[5].ToString() == "" || dr[5].ToString() == null)
                            {
                                adspent.Content = "0";
                                utilities.Content = "0";
                                misc.Content = "0";
                                total_expenses.Content = "0";
                            }
                            else
                            {
                                adspent.Content = dr[2].ToString();
                                utilities.Content = dr[3].ToString();
                                misc.Content = dr[4].ToString();
                                total_expenses.Content = dr[5].ToString();
                            }
                        });
                    }
                }
            });
        }
        public static void showExpensesGraphs(int days, CartesianChart chart)
        {
            ChartValues<ObservableValue> expensesData = new ChartValues<ObservableValue>();
            List<string> dateList = new List<string>();


            DateTime from = DateTime.Now.AddDays(-days).Date;
            DateTime to = DateTime.Now.AddDays(1).Date;
            sql.AddParam("@from", from);
            sql.AddParam("@to", to);
            sql.Query($"SELECT COALESCE(SUM(AdSpent + Utilities + Miscellaneous), 0) AS total_expenses, Date FROM tbl_expenses WHERE Date BETWEEN @from AND @to GROUP BY Date");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    expensesData.Add(new ObservableValue(double.Parse(dr[0].ToString())));
                    dateList.Add(DateTime.Parse(dr[1].ToString()).ToString());
                }

                chart.AxisX.Clear();
                chart.AxisX.Add(new Axis
                {
                    Title = "Date",
                    Labels = dateList
                });

                chart.Series.Clear();
                chart.Series.Add(new LineSeries
                {
                    Title = "Total Expenses",
                    Values = expensesData,
                });
            }
        }
        #region for importing data on table orders
        
        #endregion
    }
}
