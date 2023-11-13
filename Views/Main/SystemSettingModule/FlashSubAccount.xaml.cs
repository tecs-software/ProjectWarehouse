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
            FlashAccountDetails details = new FlashAccountDetails()
            {
                AccountName = tbAccountName.Text,
                Fullname = tbAccountName.Text,
                Mobile = tbMobile.Text,
                Email = tbEmail.Text
            };

            GlobalModel.customer_id = "BA0074";
            GlobalModel.key = "PwxNQHBeBUdLQhdbXAxzAUBqDkdKc1tSS01JQApYWH8EQWtXFBQhClMRTUZAXlZZLgYbO1ZHRXMPAkBORUJbVg==";
            await FLASH_api.FlashCreateSubaccount(tbAccountId, tbRAccountName, tbRName, details);
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
