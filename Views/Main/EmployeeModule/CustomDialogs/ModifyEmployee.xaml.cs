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
using System.Windows.Shapes;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for ModifyEmployee.xaml
    /// </summary>
    public partial class ModifyEmployee : Window
    {
        string? id;
        decimal? previousRate;

        public ModifyEmployee()
        {
            InitializeComponent();
        }

        public async void SetData(string? userId, string? firstName, string? middleName, string? lastName, string? email, string? contact)
        {
            DBHelper db = new();
            id = userId;
            tbFirstName.Text = firstName;
            tbMiddleName.Text = middleName;
            tbLastName.Text = lastName;
            tbEmail.Text = email;
            tbContact.Text = contact;
            previousRate = Converter.StringToDecimal(await db.GetValue("tbl_wage", "hourly_rate", "user_id", userId)) ;
            tbRate.Text = previousRate.ToString();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(tbFirstName, tbLastName, tbEmail, tbContact, tbRate))
            {
                MessageBox.Show("Invalid Fields");
                return;
            }

            DBHelper db = new();

            string[] columnsToUpdate = { "first_name", "middle_name", "last_name", "email", "contact_number" };
            string[] valuesToUpdate = { tbFirstName.Text, string.IsNullOrEmpty(tbMiddleName.Text) ? "N/A" : tbMiddleName.Text, tbLastName.Text, tbEmail.Text, tbContact.Text };

            if (await db.UpdateData("tbl_users", columnsToUpdate, valuesToUpdate, "user_id", id))
            {
                if (decimal.TryParse(tbRate.Text, out decimal rate))
                {
                    if (previousRate != rate)
                    {
                        if (await db.CheckIfExists("tbl_work_hours", "user_id", id, "issued", "false"))
                        {
                            MessageBox.Show("Unable to update rate per hour, there are currently un-issued hours worked");
                            return;
                        }

                        if (await db.UpdateData("tbl_wage", new string[] { "hourly_rate" }, new string[] { rate.ToString() }, "user_id", id))
                        {
                            MessageBox.Show("Employee details have been updated successfully");
                        }
                    }
                    else
                    {
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
