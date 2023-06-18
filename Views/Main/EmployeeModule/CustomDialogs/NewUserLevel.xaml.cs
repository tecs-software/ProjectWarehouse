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

namespace WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for NewUserLevel.xaml
    /// </summary>
    public partial class NewUserLevel : Window
    {
        public NewUserLevel()
        {
            InitializeComponent();
            setPermissions();
            this.SizeToContent = SizeToContent.Height;
        }

        private void setPermissions()
        {
            string[] permissionNames = {
                "Can View Dashboard",
                "Can View Inventory",
                "Can Modify Inventory",
                "Can View Order",
                "Can Modify Order",
                "Can View Employee",
                "Can Modify Employee",
                "Can View Sales"
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
    }
}
