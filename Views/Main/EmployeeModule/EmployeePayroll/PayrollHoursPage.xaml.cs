using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Database;
using WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs;

namespace WarehouseManagement.Views.Main.EmployeeModule.EmployeePayroll
{
    /// <summary>
    /// Interaction logic for PayrollHoursPage.xaml
    /// </summary>
    public partial class PayrollHoursPage : Page
    {
        public PayrollHoursPage()
        {
            InitializeComponent();
            refreshTable();
        }

        public async void refreshTable()
        {
            DBHelper db = new DBHelper();
            DataTable dataTable = await db.GetPayrollData();
            tbl_payroll.ItemsSource = dataTable.DefaultView;
        }

        private async void btnCommission_Click(object sender, RoutedEventArgs e)
        {
            if (tbl_payroll.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)tbl_payroll.SelectedItems[0];
                string userId = selectedRow["user_id"].ToString();
                string name = selectedRow["name"].ToString();

                Additionals add = new Additionals();

                add.Owner = Window.GetWindow(this); // set the parent page as the owner of the window

                add.SetData("commission", userId, name, 0);

                if (add.ShowDialog() == true)
                {
                    DBHelper db = new DBHelper();
                    
                    if (await db.InsertData("tbl_commissions", new string[] { "user_id", "commission_name", "commission_amount" }, new string[] { userId, add.description, add.amount }))
                    {
                        refreshTable();
                    }
                    
                }
            }
        }

        private async void btnHoursWorked_Click(object sender, RoutedEventArgs e)
        {
            if (tbl_payroll.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)tbl_payroll.SelectedItems[0];
                string userId = selectedRow["user_id"].ToString();
                string name = selectedRow["name"].ToString();
                decimal hoursWorked = Convert.ToDecimal(selectedRow["hours_worked"]);

                Additionals add = new Additionals();

                add.Owner = Window.GetWindow(this); // set the parent page as the owner of the window

                add.SetData( "overtime", userId, name, hoursWorked);

                if (add.ShowDialog() == true)
                {
                    DBHelper db = new DBHelper();

                    if (await db.InsertData("tbl_overtime", new string[] { "user_id", "overtime" }, new string[] { userId, add.amount }))
                    {
                       refreshTable();
                    }
                }

            }
        }

        private async void btnReimbursement_Click(object sender, RoutedEventArgs e)
        {
            if (tbl_payroll.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)tbl_payroll.SelectedItems[0];
                string userId = selectedRow["user_id"].ToString();
                string name = selectedRow["name"].ToString();

                Additionals add = new Additionals();

                add.Owner = Window.GetWindow(this); // set the parent page as the owner of the window

                add.SetData("reimbursement", userId, name, 0);

                if (add.ShowDialog() == true)
                {
                    DBHelper db = new DBHelper();

                    if (await db.InsertData("tbl_reimbursement", new string[] { "user_id", "description", "amount" }, new string[] { userId, add.description, add.amount }))
                    {
                        refreshTable();
                    }
                }
            }
        }

        private void tbl_payroll_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
