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
using WarehouseManagement.Controller;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for SystemSettingPopup.xaml
    /// </summary>
    public partial class SystemSettingPopup : Window
    {
        db_queries queries = new db_queries();
        void Clear()
        {
            txtMiscellaneous.Text = "0";
            txtAdSpent.Text = "0";
            txtUtilities.Text = "0";
        }
        void PopulateCourier(ComboBox cmb)
        {
            cmb.Items.Clear();
            cmb.Items.Add("J&T");
            cmb.Text = "J&T";
        }
        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Decimal(sender, e);
        }
        public SystemSettingPopup()
        {
            InitializeComponent();
            //code for sender information
            txtId.Text = "0";
            queries.PopulateShop(cmbAction);
            //code for courier information
            PopulateCourier(cmbCourier);

            txtMiscellaneous.Text = "0";
            txtAdSpent.Text = "0";
            txtUtilities.Text = "0";

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            queries.province(cmbProvince);
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            switch (tabItem)
            {
                case "Sender Information":
                    //senderInfoFrame.Source = new Uri("../ProductView/ProductInformation.xaml", UriKind.Relative);
                    break;
                case "Courier Accounts":
                   // courierAccountFrame.Source = new Uri("../ProductView/ProductList.xaml", UriKind.Relative);
                    break;
                case "Import Address":
                    importAddressFrame.Source = new Uri("../SystemSettingModule/FrameImportAddress.xaml", UriKind.Relative);
                    break;
                default:
                    return;
            }
        }

        private void cmbCity_DropDownClosed(object sender, EventArgs e)
        {
            cmbBarangay.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.baranggay(cmbBarangay, cmbCity.Text);
        }

        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cmbCity.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.city(cmbCity, cmbProvince.Text);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.Instance.ModuleAccessList.Contains("Modify Shop/Pages"))
            {
                MessageBox.Show("You don't have enough permission to add or update shop/pages");
                return;
            }

            if (Util.IsAnyTextBoxEmpty(txtAddress, txtPagename, txtPhone) || Util.IsAnyComboBoxItemEmpty(cmbProvince) || Util.IsAnyComboBoxItemEmpty(cmbCity) || Util.IsAnyComboBoxItemEmpty(cmbBarangay))
            {
                MessageBox.Show("Please complete all required fields on sender information.");
                return;
            }
            else
            {
                if (db_queries.checkExistingShop(txtPagename.Text) && btnSubmit_sender.Content.ToString() != "UPDATE")
                {
                    MessageBox.Show("Shop already exists");
                    return;
                }
                else
                {
                    if (queries.insert_sender(txtId.Text, txtPagename, txtPhone, cmbProvince, cmbCity, cmbBarangay, txtAddress))
                    {
                        MessageBox.Show("Shop/Page Save");
                        txtAddress.Clear();
                        txtPagename.Clear();
                        txtId.Text = "0";
                        btnSubmit_sender.Content = "ADD";
                        txtPhone.Clear();
                        cmbProvince.Text = "";
                        cmbCity.Text = "";
                        cmbBarangay.Text = "";

                        queries.PopulateShop(cmbAction);
                        cmbAction.SelectedIndex = -1;
                    }
                    else
                    {
                        return;
                    }
                }
            }

        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void txtAddress_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (HasSymbols(e.Text))
            {
                e.Handled = true; // Ignore the input
            }
        }
        private bool HasSymbols(string text)
        {
            foreach (char c in text)
            {
                // Check if the character is not alphanumeric, space, ",", ".", or "-"
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != ',' && c != '.' && c != '-')
                {
                    return true;
                }
            }
            return false;
        }

        private void btnImportAddress_Click(object sender, RoutedEventArgs e)
        {

        }
       
        private void btnConfirmExpenses_Click(object sender, RoutedEventArgs e)
        {
            if (txtAdSpent.Text == "" || txtUtilities.Text == "" || txtMiscellaneous.Text == "")
                MessageBox.Show("Please complete all fields");
            else
            {
                Expenses expenses = new Expenses() {
                    UserID = CurrentUser.Instance.userID,
                    AdSpent = decimal.Parse(txtAdSpent.Text),
                    Utilities = decimal.Parse(txtUtilities.Text),
                    Miscellaneous = decimal.Parse(txtMiscellaneous.Text),
                };
                ExpensesController.InsertExpenses(expenses);
                MessageBox.Show("Data Inserted Successfully.");
                Clear();
            }
        }

        private void cmbAction_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbAction.SelectedIndex != -1)
            {
                if (cmbAction.Text.ToUpper() == "ADD")
                {
                    txtAddress.Clear();
                    txtPagename.Clear();
                    txtId.Text = "0";
                    btnSubmit_sender.Content = "ADD";
                    txtPhone.Clear();
                    cmbProvince.Text = "";
                    cmbCity.Text = "";
                    cmbBarangay.Text = "";
                }
                else
                {
                    queries.DisplaySender(cmbAction.Text, this);
                    btnSubmit_sender.Content = "UPDATE";
                }
            }
        }

        private void btnSubmitCourier_Click(object sender, RoutedEventArgs e)
        {
            if (txtCustomerID.Text == "" || txtEccompanyId.Text == "")
                MessageBox.Show("Please complete all fields");
            else
            {
                GlobalModel.eccompany_id = txtEccompanyId.Text;
                GlobalModel.customer_id = txtCustomerID.Text;
                queries.UpdateCourier(cmbCourier.Text, txtCustomerID.Text, txtEccompanyId.Text);
                MessageBox.Show("Credentials has been updated.");
                txtEccompanyId.Text = "";
                cmbCourier.SelectedIndex = -1;
                txtCustomerID.Text = "";
            }

        }
    }
}
