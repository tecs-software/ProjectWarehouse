using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WarehouseManagement.Controller;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for ModifyEmployee.xaml
    /// </summary>
    public partial class ModifyEmployee : Window
    {
        string? id;
        decimal? previousRate;
        string? prevUsername;
        string? prevEmail;
        string? prevRole;
        

        public ModifyEmployee()
        {
            InitializeComponent();
            UserController.LoadSender(cmbSellerName);
            InitializeControls();


        }

        private async void InitializeControls()
        {
            DBHelper db = new DBHelper();
            List<Roles> roles = await db.GetRoles();

            cbRole.ItemsSource = roles;
            cbRole.DisplayMemberPath = "roleName";
        }


        public async void SetData(string? userId, string? firstName, string? middleName, string? lastName, string? email, string? contact, string? shopName, string? username, string? role)
        {
            DBHelper db = new();
            id = userId;
            tbFirstName.Text = firstName;
            tbUsername.Text = username;
            prevUsername = username;
            prevEmail = email;
            tbMiddleName.Text = middleName;
            tbLastName.Text = lastName;
            tbEmail.Text = email;
            tbContact.Text = contact;
            previousRate = Converter.StringToDecimal(await db.GetValue("tbl_wage", "hourly_rate", "user_id", userId)) ;
            tbRate.Text = previousRate.ToString();
            prevRole = role;
            cbRole.Text = role;
            cmbSellerName.Text = UserController.GetSenderName(id);
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DBHelper db = new();

            if (Util.IsAnyTextBoxEmpty(tbFirstName, tbLastName, tbEmail, tbContact, tbRate, tbUsername))
            {
                MessageBox.Show("Invalid Fields");
                return;
            }

            if(prevUsername != tbUsername.Text.Trim())
            {
                if (await db.IsDataExistsAsync("tbl_users", "username", tbUsername.Text.Trim()))
                {
                    MessageBox.Show("Username already exists!");
                    return;
                }
            }


            if (tbPassword.Password.Length > 0)
            {
                if (tbPassword.Password.Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters long.");
                    return;
                }
            }

            SecureString newPassword = Util.PBtoSecureString(tbPassword);


            string[] columnsToUpdate = { "first_name", "middle_name", "last_name", "username","email","password", "contact_number" };
            string[] valuesToUpdate = { tbFirstName.Text, string.IsNullOrEmpty(tbMiddleName.Text) ? "N/A" : tbMiddleName.Text, tbLastName.Text, tbUsername.Text, tbEmail.Text, Util.HashPassword(newPassword), tbContact.Text };

            if (await db.UpdateData("tbl_users", columnsToUpdate, valuesToUpdate, "user_id", id))
            {
                if (decimal.TryParse(tbRate.Text, out decimal rate))
                {

                    if (prevRole != cbRole.Text)
                    {
                        if (cbRole.SelectedItem != null)
                        {
                            Roles selectedRole = (Roles)cbRole.SelectedItem;
                            int roleId = selectedRole.roleID;

                            if(!await db.UpdateData("tbl_access_level", new string[] { "role_id" }, new string[] { roleId.ToString() }, "user_id", id))
                            {
                                MessageBox.Show("Failed to update user level");
                            }
                        }
                    }

                    if (previousRate != rate)
                    {
                        if (await db.CheckIfExists("tbl_work_hours", "user_id", id, "issued", "false"))
                        {
                            var confirmationResult = MessageBox.Show("There are currently un-issued hours worked. Do you still want to update the rate per hour?", "Confirmation", MessageBoxButton.YesNo);
                            if (confirmationResult == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        if (await db.UpdateData("tbl_wage", new string[] { "hourly_rate" }, new string[] { rate.ToString() }, "user_id", id))
                        {
                            //Update SenderID
                            UserController.UpdateSender(id, cmbSellerName.Text);
                            MessageBox.Show("Employee details have been updated successfully");
                        }
                    }
                    else
                    {       
                        UserController.UpdateSender(id, cmbSellerName.Text);
                        MessageBox.Show("Employee details have been updated successfully");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid rate per hour");
                }
            }
        }
    }
}
