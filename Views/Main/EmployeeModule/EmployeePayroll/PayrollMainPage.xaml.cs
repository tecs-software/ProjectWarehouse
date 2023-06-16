using System;
using System.Collections.Generic;
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
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.EmployeeModule.EmployeePayroll
{
    /// <summary>
    /// Interaction logic for PayrollMainPage.xaml
    /// </summary>
    public partial class PayrollMainPage : Page
    {
        private PayrollHoursPage payrollHours = new PayrollHoursPage();
        private PayrollReviewPage payrollReview = new PayrollReviewPage();
        private PayrollPayslipPage payrollPayslip = new PayrollPayslipPage();
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
                btnSaveAndContinue.Content = "PRINT";
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
                case 2:
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
                // Perform appropriate action for next button in the last page
            }


        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
