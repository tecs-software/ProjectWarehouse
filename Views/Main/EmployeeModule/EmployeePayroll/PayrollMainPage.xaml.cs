using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.EmployeeModule.EmployeePayroll
{
    /// <summary>
    /// Interaction logic for PayrollMainPage.xaml
    /// </summary>
    public partial class PayrollMainPage : Page
    {
        private PayrollHoursPage payrollHours;
        private PayrollReviewPage payrollReview;
        private PayrollPayslipPage payrollPayslip;
        private int currentPageIndex = 0;
        private List<Page> pages = new List<Page>();

        public PayrollMainPage()
        {
            InitializeComponent();
            SetupPages();
            SetPage(0);
        }

        private void SetupPages()
        {
            DBHelper db = new DBHelper();
            db.DeleteInvalidRecords();
            payrollHours = new PayrollHoursPage();
            payrollReview = new PayrollReviewPage();
            payrollPayslip = new PayrollPayslipPage();
            pages.Add(payrollHours);
            pages.Add(payrollReview);
            pages.Add(payrollPayslip);
        }

        private async void SetPage(int index)
        {
            DBHelper db = new DBHelper();

            if (index == 0)
            {
                mainFrame.Navigate(pages[index]);
                currentPageIndex = index;
                payslipControls.Visibility = Visibility.Collapsed;
                print.Visibility = Visibility.Collapsed;
                btnSaveChangesForLater.Content = "SAVE CHANGES FOR LATER";
                btnSaveAndContinue.Content = "SAVE & CONTINUE";
                pbConfirmation.Value = 0;
                pbReview.Value = 0;
                pbHours.Value = 100;
            }else if(index == 1)
            {
                await db.SavePayrollChanges();
                mainFrame.Navigate(pages[index]);
                currentPageIndex = index;
                payslipControls.Visibility = Visibility.Collapsed;
                print.Visibility = Visibility.Collapsed;
                btnSaveChangesForLater.Content = "BACK";
                btnSaveAndContinue.Content = "NEXT";
                pbConfirmation.Value = 0;
                pbHours.Value = 0;
                pbReview.Value = 100;
            }
            else if(index == 2)
            {
                mainFrame.Navigate(pages[index]);
                currentPageIndex = index;
                payslipControls.Visibility = Visibility.Visible;
                List<User>? users = await db.GetUsers();

                cbEmployee.ItemsSource = users;
                cbEmployee.DisplayMemberPath = "name";
                
                print.Visibility = Visibility.Visible;
                btnSaveAndContinue.Content = "ISSUE PAYSLIP";
                pbReview.Value = 0;
                pbConfirmation.Value = 100;
            }
        }

        private async void btnSaveChangesForLater_Click(object sender, RoutedEventArgs e)
        {
            DBHelper db = new DBHelper();

            switch (currentPageIndex)
            {
                case 0:
                    if (await db.SavePayrollChanges())
                    {
                        MessageBox.Show("Changes saved successfully");
                    }
                    break;
                case 1:
                    SetPage(currentPageIndex - 1);
                    break;
                case 2:
                    payrollPayslip = new PayrollPayslipPage();
                    SetPage(currentPageIndex - 1);
                    break;

            }


        }

        private void btnSaveAndContinue_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < pages.Count - 1)
            {
                if (currentPageIndex == 0 && pbHours.Value == 100)
                {
                    SetPage(currentPageIndex + 1);
                }
                else if (currentPageIndex == 1 && pbReview.Value == 100)
                {
                    SetPage(currentPageIndex + 1);
                }
            }
            else
            {
                if (cbEmployee.SelectedItem != null && cbEmployee.SelectedItem.ToString().ToLower() != "select employee")
                {
                    var confirmationResult = MessageBox.Show("Are you sure you want to reset the employee's payroll data?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirmationResult == MessageBoxResult.Yes)
                    {
                        DBHelper db = new DBHelper();
                        int userID = ((User)cbEmployee.SelectedItem).userID;
                        db.IssuePayslip(userID);

                        var selectedUser = (User)cbEmployee.SelectedItem;
                        var users = (List<User>)cbEmployee.ItemsSource;
                        users.Remove(selectedUser);

                        cbEmployee.ItemsSource = null;
                        cbEmployee.ItemsSource = users;

                        payrollHours.refreshTable();
                        payrollReview.refreshTable();
                    }
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = cbEmployee.SelectedIndex;

            // Get the total number of items in the ComboBox
            int totalItems = cbEmployee.Items.Count;

            // Calculate the index of the next item
            int nextIndex = (currentIndex + 1) % totalItems;

            // Set the selected index to the next item
            cbEmployee.SelectedIndex = nextIndex;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = cbEmployee.SelectedIndex;

            // Get the total number of items in the ComboBox
            int totalItems = cbEmployee.Items.Count;

            // Calculate the index of the next item
            int previousIndex = (currentIndex - 1 + totalItems) % totalItems;

            // Set the selected index to the next item
            cbEmployee.SelectedIndex = previousIndex;
        }

        private async void cbEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = "", userLevel = null;
            decimal hourlyRate = 0, overtime = 0, commissions = 0, incentive = 0, hoursWorked = 0, reimbursements = 0, basicPay = 0, additionalPay = 0, deductions = 0;

            if (cbEmployee.SelectedItem != null)
            {
                DBHelper db = new DBHelper();

                int userId = ((User)cbEmployee.SelectedItem).userID;
                name = ((User)cbEmployee.SelectedItem).name;
                var userData = await db.GetUserFinancialData(userId);

                if (userData != null)
                {
                    foreach (var dictionary in userData)
                    {
                        foreach (var keyValuePair in dictionary)
                        {
                            string key = keyValuePair.Key;
                            object value = keyValuePair.Value;

                            if (value != DBNull.Value)
                            {
                                switch (key)
                                {
                                    case "incentive":
                                        incentive = Convert.ToDecimal(value);
                                        break;
                                    case "role_name":
                                        userLevel = value.ToString();
                                        break;
                                    case "hourly_rate":
                                        hourlyRate = Convert.ToDecimal(value);
                                        break;
                                    case "overtime":
                                        overtime = Convert.ToDecimal(value);
                                        break;
                                    case "commission":
                                        commissions = Convert.ToDecimal(value);
                                        break;
                                    case "reimbursement":
                                        reimbursements = Convert.ToDecimal(value);
                                        break;
                                    case "hours_worked":
                                        hoursWorked = Convert.ToDecimal(value);
                                        break;
                                    case "deductions":
                                        deductions = Convert.ToDecimal(value);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            basicPay = hoursWorked * hourlyRate;
            additionalPay = (overtime * 60) + commissions + incentive;


            var pp = mainFrame.Content as PayrollPayslipPage;
            pp?.setData(name, userLevel, basicPay, additionalPay, reimbursements, deductions);
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DBHelper db = new DBHelper();

            if (await db.HasInvalidPayrollRecords())
            {
                MessageBoxResult result = MessageBox.Show("There are unsave changes in payroll, do you want to save changes?", "Confirmation", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        await db.SavePayrollChanges();
                        break;
                    case MessageBoxResult.No:

                        break;
                }
            }
            
        }

        private void print_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
