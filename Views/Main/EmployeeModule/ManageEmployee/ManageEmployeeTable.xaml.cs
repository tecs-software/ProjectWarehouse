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
using WarehouseManagement.Helpers;
using WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs;

namespace WarehouseManagement.Views.Main.EmployeeModule.ManageEmployee
{
    /// <summary>
    /// Interaction logic for ManageEmployeeTable.xaml
    /// </summary>
    public partial class ManageEmployeeTable : Page
    {
        public ManageEmployeeTable()
        {
            InitializeComponent();
            PopulateDataGrid();
        }

        private async void PopulateDataGrid()
        {
            await ShowEmployees();
        }

        public async Task ShowEmployees()
        {
            string query = @"SELECT u.user_id, u.first_name, u.middle_name, u.last_name, u.email, u.username, u.contact_number, u.status, r.role_name
                FROM tbl_users u
                LEFT JOIN tbl_access_level a ON u.user_id = a.user_id
                LEFT JOIN tbl_roles r ON a.role_id = r.role_id
                WHERE u.username <> ''";

            DataTable? dataTable = await DBHelper.GetTable(query);

            if (dataTable != null)
            {
                dataTable.Columns.Add("name", typeof(string), "last_name + ', ' + first_name + ' ' + IIF(middle_name = 'N/A', '', SUBSTRING(middle_name, 1, 1) + '.')");

                DataView dataView = new DataView(dataTable);
                tblUsers.ItemsSource = dataView;
            }
            else
            {
                MessageBox.Show("Failed to retrieve products, database error.");
            }
        }

        public async Task ShowActiveEmployees()
        {
            try
            {
                using (DBHelper db = new DBHelper())
                {
                    DataTable dataTable = await db.GetUsersDataTable(@"au.login_time IS NOT NULL AND au.logout_time IS NULL
                                                               AND u.username <> ''
                                                               AND u.status = 'Enabled'");

                    DataColumn nameColumn = new DataColumn("name", typeof(string), "last_name + ', ' + first_name + ' ' + IIF(middle_name = 'N/A', '', SUBSTRING(middle_name, 1, 1) + '.')");
                    dataTable.Columns.Add(nameColumn);

                    tblUsers.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve employees: {ex.Message}", "Database Error");
            }
        }

        public async Task ShowInactiveEmployees()
        {
            try
            {
                using (DBHelper db = new DBHelper())
                {
                    DataTable dataTable = await db.GetUsersDataTable(@"(au.login_time IS NOT NULL AND au.logout_time IS NOT NULL OR au.login_time IS NULL AND au.logout_time IS NULL)
                                                               AND u.username <> ''
                                                               AND u.status = 'Enabled'");

                    DataColumn nameColumn = new DataColumn("name", typeof(string), "last_name + ', ' + first_name + ' ' + IIF(middle_name = 'N/A', '', SUBSTRING(middle_name, 1, 1) + '.')");
                    dataTable.Columns.Add(nameColumn);

                    tblUsers.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve employees: {ex.Message}", "Database Error");
            }
        }

        public async Task ShowDisabledEmployees()
        {
            try
            {
                using (DBHelper db = new DBHelper())
                {
                    DataTable dataTable = await db.GetUsersDataTable(@"u.username <> ''
                                                               AND u.status = 'Disabled'");

                    DataColumn nameColumn = new DataColumn("name", typeof(string), "last_name + ', ' + first_name + ' ' + IIF(middle_name = 'N/A', '', SUBSTRING(middle_name, 1, 1) + '.')");
                    dataTable.Columns.Add(nameColumn);

                    tblUsers.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve employees: {ex.Message}", "Database Error");
            }
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (tblUsers.SelectedItem == null)
                return;

            string? status = ((DataRowView)tblUsers.SelectedItem)["status"].ToString();
            MenuItem item1 = (status == "Disabled") ? new MenuItem() { Header = "Reactivate" } : new MenuItem() { Header = "Archive" };
            MenuItem item2 = new MenuItem() { Header = "Manage Employee" };

            Util.ShowContextMenuForButton(sender as Button, item1, item2);
            item1.Click += Archive_Click;
            item2.Click += ManageEmployee_Click;
        }

        private async void Archive_Click(object sender, RoutedEventArgs e)
        {
            if (tblUsers.SelectedItem == null)
                return;

            DBHelper db = new DBHelper();
            DataRowView selectedRow = (DataRowView)tblUsers.SelectedItem;
            string? id = selectedRow["user_id"].ToString();
            string? status = selectedRow["status"].ToString();

            string newStatus = (status == "Disabled") ? "Enabled" : "Disabled";
            string actionMessage = (status == "Disabled") ? "reactivated" : "archived";

            if (newStatus == "Disabled" && await db.CheckIfLogged("tbl_active_users", "user_id", id, "logout_time"))
            {
                MessageBox.Show("Employee is currently logged in, unable to disable");
                return;
            }

            if (await db.UpdateData("tbl_users", new string[] { "status" }, new string[] { newStatus }, "user_id", id))
            {
                MessageBox.Show($"Employee {actionMessage}");
                await ShowEmployees();
            }
        }

        private void ManageEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (tblUsers.SelectedItems.Count > 0)
            {
                DataRowView? selectedRow = (DataRowView) tblUsers.SelectedItems[0];

                if (selectedRow == null) 
                    return;

                string? userId = selectedRow["user_id"].ToString();
                string? fname = selectedRow["first_name"].ToString();
                string? mname = selectedRow["middle_name"].ToString();
                string? lname = selectedRow["last_name"].ToString();
                string? email = selectedRow["email"].ToString();
                string? contact = selectedRow["contact_number"].ToString();

                ModifyEmployee em = new ModifyEmployee();

                em.Owner = Window.GetWindow(this);

                em.SetData(userId, fname, mname, lname, email, contact);

                em.ShowDialog();
            }
        }
    }
}
