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
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs;
using WarehouseManagement.Views.Main.EmployeeModule.EmployeePayroll;
using WarehouseManagement.Views.Main.EmployeeModule.ManageEmployee;
using MenuItem = WarehouseManagement.Models.MenuItem;

namespace WarehouseManagement.Views.Main.EmployeeModule
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : Page
    {
        private ManageEmployeeTable employee = new ManageEmployeeTable();
        private PayrollMainPage payroll = new PayrollMainPage();

        public EmployeeView()
        {
            InitializeComponent();
            showEmployeeMenu();
            Employee();
        }

        public void Employee(string type = null)
        {
            btnGenerateAuthen.Visibility = Visibility.Visible;
            mainFrame.Navigate(employee);
            //employee?.showEmployeeTable();
        }

        public void Payroll()
        {
            btnGenerateAuthen.Visibility = Visibility.Collapsed;
            mainFrame.Navigate(payroll);
            //payroll?.showPayrollHours();
        }


        public void showEmployeeMenu()
        {
            //(int active, int inactive, int disabled) counts = DBHelper.GetUserCounts();

            //int activeCount = counts.active;
            //int inactiveCount = counts.inactive;
            //int disabledCount = counts.disabled;

            var menuEmployee = new List<SubMenuItem>();

            menuEmployee.Add(new SubMenuItem("Active", 0));
            menuEmployee.Add(new SubMenuItem("Offline", 0));
            menuEmployee.Add(new SubMenuItem("Archived", 0));

            var menuPayroll = new List<SubMenuItem>();

            var employee = new MenuItem("PEOPLE", menuEmployee);
            var payroll = new MenuItem("RUN PAYROLL", menuPayroll);

            // Remove all child controls from the Menu control
            Menu.Children.Clear();
            Menu.Children.Add(new EmployeeMenu(employee));
            Menu.Children.Add(new EmployeeMenu(payroll));
        }

        private void btnGenerateAuthen_Click(object sender, RoutedEventArgs e)
        {
            GenerateAuthentication ga = new GenerateAuthentication();

            ga.Owner = Window.GetWindow(this); // set the parent page as the owner of the window

            if (ga.ShowDialog() == true)
            {
                // handle the result of the dialog
            }
        }
    }
}
