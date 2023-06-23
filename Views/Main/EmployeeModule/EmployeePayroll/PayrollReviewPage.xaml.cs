using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace WarehouseManagement.Views.Main.EmployeeModule.EmployeePayroll
{
    /// <summary>
    /// Interaction logic for PayrollReviewPage.xaml
    /// </summary>
    public partial class PayrollReviewPage : Page
    {
        public PayrollReviewPage()
        {
            InitializeComponent();
           
        }
        public async void refreshTable()
        {
            DBHelper db = new DBHelper();

            DataTable dataTable = await db.GetPayrollData();

            dataTable.Columns.Add("commission_details");
            dataTable.Columns.Add("commission_amount");
            dataTable.Columns.Add("reimbursement_details");
            dataTable.Columns.Add("reimbursement_values");


            foreach (DataRow row in dataTable.Rows)
            {
                string userId = row["user_id"].ToString();

                Dictionary<string, double> commission = await db.GetCommissionDataForUser(userId);
                Dictionary<string, double> reimbursement = await db.GetReimbursementsForUser(userId);

                CultureInfo phCulture = new CultureInfo("en-PH");

                StringBuilder commissionKeys = new StringBuilder();
                StringBuilder commissionvalues = new StringBuilder();

                StringBuilder reimbursementKeys = new StringBuilder();
                StringBuilder reimbursementValues = new StringBuilder();

                foreach (KeyValuePair<string, double> kvp in commission)
                {
                    commissionKeys.Append("+ " + kvp.Key + "\n");
                    commissionvalues.Append(kvp.Value.ToString("C", phCulture) + "\n");
                }

                foreach (KeyValuePair<string, double> kvp in reimbursement)
                {
                    reimbursementKeys.Append("+ " + kvp.Key + "\n");
                    reimbursementValues.Append(kvp.Value.ToString("C", phCulture) + "\n");
                }

                row["commission_details"] = commissionKeys;
                row["commission_amount"] = commissionvalues;

                row["reimbursement_details"] = reimbursementKeys;
                row["reimbursement_values"] = reimbursementValues;
            }

            tb_payroll_review.ItemsSource = dataTable.DefaultView;
        }

        private void btnDeductions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tb_payroll_review_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            refreshTable();
        }
    }
}
