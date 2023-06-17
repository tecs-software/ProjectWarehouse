using System;
using System.Collections.Generic;
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

namespace WarehouseManagement.Views.Main.EmployeeModule.EmployeePayroll
{
    /// <summary>
    /// Interaction logic for PayrollPayslipPage.xaml
    /// </summary>
    public partial class PayrollPayslipPage : Page
    {
        public PayrollPayslipPage()
        {
            InitializeComponent();
        }

        public void setData(string name, string user_level, decimal basicPay, decimal additionalPay, decimal reimbursement, decimal deductions)
        {
            var culturePH = new CultureInfo("en-PH");
            lbName.Text = name;
            lbDesignation.Text = user_level;
            lbBasicPay.Text = "₱" + basicPay.ToString("n2", culturePH);
            lbAdditionalPay.Text = "₱" + additionalPay.ToString("n2", culturePH);
            lbReimbursement.Text = "₱" + reimbursement.ToString("n2", culturePH);
            lbDeductions.Text = "₱" + deductions.ToString("n2", culturePH);
            lbTotalAddition.Text = "₱" + (basicPay + additionalPay + reimbursement).ToString("n2", culturePH);
            lbTotalDeductions.Text = "₱" + deductions.ToString("n2", culturePH);
            tbNetPay.Text = "₱" + (basicPay + additionalPay + reimbursement - deductions).ToString("n2", culturePH);
            lbDate.Text = DateTime.Today.ToString("MMMM dd, yyyy");

        }
    }
}
