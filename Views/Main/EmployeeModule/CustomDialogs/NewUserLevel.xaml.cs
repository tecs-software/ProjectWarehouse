using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for NewUserLevel.xaml
    /// </summary>
    public partial class NewUserLevel : Window
    {
        private Roles? role;
        private string id = null;
        private bool isUpdate = false;

        public NewUserLevel(string? id = null)
        {
            InitializeComponent();
            setPermissions();

            if (id != null)
            {
                lblTitle.Text = "Modify Role";
                isUpdate = true;
                this.id = id;
                setData(id);
            }

            this.SizeToContent = SizeToContent.Height;
        }

        private async void setData(string id)
        {
            DBHelper db = new DBHelper();

            role = new Roles();
            role = await db.GetRole(id);

            if (role != null)
            {
                tbRoleName.Text = role.roleName;
                tbHourlyRate.Text = (role.hourlyRate <= 0) ? "N/A" : role.hourlyRate.ToString();
            }

            // Retrieve permissions from tbl_module_access using role_id
            List<String> permissions = await db.GetModuleAccess(id);

            foreach (CheckBox checkbox in permissionsCheckbox.Children.OfType<CheckBox>())
            {
                if (permissions.Contains(checkbox.Content.ToString()))
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void setPermissions()
        {
            string[] permissionNames = {
                "View Dashboard",
                "View Inventory",
                "Modify Inventory",
                "View Order",
                "Modify Order",
                "View Employee",
                "Modify Employee",
                "Modify Order Inquiry",
                "View Order Inquiry",
                "Modify Shop/Pages",
                "View Shop/Pages",
                "View Suspicious Order",
                "Modify System Settings"
            };

            foreach (string permissionName in permissionNames)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = permissionName;
                checkbox.IsChecked = false;
                checkbox.FontSize = 15;

                // Set the checked color and checkmark color of the checkbox
                var checkBoxStyle = new Style(typeof(CheckBox));
                var trigger = new Trigger { Property = ToggleButton.IsCheckedProperty, Value = true };
                trigger.Setters.Add(new Setter(Control.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#04B5AD"))));
                trigger.Setters.Add(new Setter(Control.ForegroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#3c3c3c")))); // Set foreground color
                checkBoxStyle.Triggers.Add(trigger);
                checkbox.Style = checkBoxStyle;

                permissionsCheckbox.Children.Add(checkbox);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(tbRoleName))
            {
                MessageBox.Show("Please enter role name");
                return;
            }

            if (tbRoleName.Text.ToLower().Equals("admin"))
            {
                MessageBox.Show("Admin is not allowed");
                return;
            }

            DBHelper db = new DBHelper();

            if (!isUpdate)
            {
                if (await db.IsDataExistsAsync("tbl_roles", "role_name", tbRoleName.Text))
                {
                    MessageBox.Show("Role already exists!");
                    return;
                }
            }

            string roleName = tbRoleName.Text;
            decimal hourlyRate = Converter.StringToDecimal(tbHourlyRate.Text);

            List<string> moduleAccessList = new List<string>();
            foreach (CheckBox checkbox in permissionsCheckbox.Children)
            {
                if (checkbox.IsChecked == true)
                {
                    moduleAccessList.Add(checkbox.Content.ToString());
                }
            }

            bool success = await db.InsertOrUpdateRole(roleName, hourlyRate, moduleAccessList, id);

            if (success)
            {
                this.DialogResult = true;
            }
            else
            {
                if (isUpdate)
                {
                    MessageBox.Show("Failed to update role.");
                }
                else
                {
                    MessageBox.Show("Failed to add role");
                }
            }
        }
    }
}
