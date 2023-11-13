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

            //code for courier information
            PopulateCourier(cmbCourier);
            rdbJandT.IsChecked = true;
            rdbFlash.IsChecked = false;
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
            queries.FlashProvince(cmbProvinceFlash);
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            switch (tabItem)
            {
               
                case "Import Address":
                    importAddressFrame.Source = new Uri("../SystemSettingModule/FrameImportAddress.xaml", UriKind.Relative);
                    break;
                case "Waybill Journal":
                    WaybillJournalFrame.Source = new Uri("../SystemSettingModule/WaybillJournal.xaml", UriKind.Relative);
                    break;
                case "Create Flash Sub-account":
                    FlashSubaccountFrame.Source = new Uri("../SystemSettingModule/FlashSubAccount.xaml", UriKind.Relative);
                    break;
                case "Printer Setting":
                    PrinterSettingFrame.Source = new Uri("../SystemSettingModule/FramePrinterSetting.xaml", UriKind.Relative);
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
                    if (queries.insert_sender(txtId.Text, txtPagename, txtPhone, cmbProvince, cmbCity, cmbBarangay, txtAddress, "", 1))
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
        void CheckedRadio(string text)
        {
            if (text == "J&T")
            {
                ContainerFlash.Visibility = Visibility.Collapsed;
                ContainerJnT.Visibility = Visibility.Visible;
                cmbProvinceFlash.Text = "";

            }
            if (text == "FLASH")
            {
                ContainerFlash.Visibility = Visibility.Visible;
                ContainerJnT.Visibility = Visibility.Collapsed;
                cmbProvince.Text = "";
            }
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

        private void rdbJandT_Checked(object sender, RoutedEventArgs e)
        {
            txtAddress.Clear();
            txtPagename.Clear();
            txtPhone.Clear();
            cmbBarangayFlash.Text = "";
            cmbPostalCodeFlash.Text = "";
            CheckedRadio(rdbJandT.Content.ToString());
            queries.PopulateShop(cmbAction, 1);
        }

        private void rdbFlash_Checked(object sender, RoutedEventArgs e)
        {
            txtAddress.Clear();
            txtPagename.Clear();
            txtPhone.Clear();
            CheckedRadio(rdbFlash.Content.ToString());
            queries.PopulateShop(cmbAction, 2);
        }

        private void cmbProvinceFlash_DropDownClosed(object sender, EventArgs e)
        {
            cmbCityFlash.SelectedIndex = -1;
            queries.FlashCity(cmbCityFlash, cmbProvinceFlash.Text);
        }

        private void cmbCityFlash_DropDownClosed(object sender, EventArgs e)
        {
            queries.FlashBaranggay(cmbBarangayFlash, cmbCityFlash.Text);
        }

        private void cmbBarangayFlash_DropDownClosed(object sender, EventArgs e)
        {
            queries.FlashPostalCode(cmbPostalCodeFlash, cmbBarangayFlash.Text);
        }

        private void cmbBarangayFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbPostalCodeFlash.SelectedIndex = -1;
        }

        private void cmbProvinceFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCityFlash.SelectedIndex = -1;
            cmbBarangayFlash.ItemsSource = null;
        }

        private void cmbCityFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbBarangayFlash.SelectedIndex = -1;
        }

        private void txtPagename_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemQuotes)
            {
                // Suppress the key event to prevent the character from being entered
                e.Handled = true;
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemQuotes)
            {
                // Suppress the key event to prevent the character from being entered
                e.Handled = true;
            }
        }

        private void txtPagename_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Contains("'"))
            {
                // Suppress the event if the pasted text contains a single quote
                e.Handled = true;
            }
        }
    }
}
