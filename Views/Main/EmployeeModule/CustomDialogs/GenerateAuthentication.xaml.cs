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
    /// Interaction logic for GenerateAuthentication.xaml
    /// </summary>
    public partial class GenerateAuthentication : Window
    {
        public GenerateAuthentication()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControls();
        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {

            if (cbUserLevel.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a user level");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to generate the authentication code?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DBHelper db = new DBHelper();

                Dictionary<string, object> firstTableValues = new Dictionary<string, object>
                {
                    { "authentication_code", tbAuthen.Text },
                    { "status", "Enabled" },
                };

                Dictionary<string, object> secondTableValues = new Dictionary<string, object>
                {
                    { "access_level", cbUserLevel.Text },
                };

                Dictionary<string, object> thirdTableValues = new Dictionary<string, object>
                {
                    { "hourly_rate", tbRate.Text },
                };

                if (await db.InsertDataWithForeignKey("tbl_users", firstTableValues, "user_id", "tbl_user_access", secondTableValues, "tbl_wage", thirdTableValues))
                {
                    Clipboard.SetText(tbAuthen.Text);
                    cbUserLevel.SelectedIndex = -1;
                    tbAuthen.Text = string.Empty;
                    tbRate.Text = string.Empty;
                    MessageBox.Show("Authentication code generated successfully, code copied to clipboard", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        private void cbUserLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbUserLevel.SelectedIndex >= 0)
            {
                tbAuthen.Text = Util.GenerateRandomString(10);
                UpdateRateTextBox();
            }
        }

        private void UpdateRateTextBox()
        {
            if (cbUserLevel.Text == "Warehouse Manager")
            {
                tbRate.Text = "60.00";
            }
            else if (cbUserLevel.Text == "Sales Agent")
            {
                tbRate.Text = "50.00";
            }
        }

        private void InitializeControls()
        {
            cbUserLevel.SelectedIndex = 0;
            UpdateRateTextBox();
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Decimal(sender, e);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
