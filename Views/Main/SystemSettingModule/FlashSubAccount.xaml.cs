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
using WarehouseManagement.Controller;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for FlashSubAccount.xaml
    /// </summary>
   
    public partial class FlashSubAccount : Page
    {
        static sql_control sql = new sql_control();
        public FlashSubAccount()
        {
            InitializeComponent();
        }

        private void tbAccountName_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            int FlashCounter = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_couriers WHERE courier_name = 'FLASH'"));

            if(Util.IsAnyTextBoxEmpty(tbAccountName, tbName, tbMobile, tbEmail))
            {
                MessageBox.Show("Complete the details to proceed.");
            }
            else
            {
                if (FlashCounter > 0)
                {
                    MessageBox.Show("Creation Denied! There's already existing FLASH account.");
                }
                else
                {
                    FlashAccountDetails details = new FlashAccountDetails()
                    {
                        AccountName = tbAccountName.Text,
                        Fullname = tbName.Text,
                        Mobile = tbMobile.Text,
                        Email = tbEmail.Text
                    };

                    GlobalModel.customer_id = "BA0074";
                    GlobalModel.key = "PwxNQHBeBUdLQhdbXAxzAUBqDkdKc1tSS01JQApYWH8EQWtXFBQhClMRTUZAXlZZLgYbO1ZHRXMPAkBORUJbVg==";
                    await FLASH_api.FlashCreateSubaccount(details, "FLASH");

                    tbAccountName.Clear();
                    tbMobile.Clear();
                    tbEmail.Clear();
                    tbName.Clear();
                }
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

        private void tbMobile_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }
    }
}
