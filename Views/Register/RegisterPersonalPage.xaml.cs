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
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Views.Register
{
    /// <summary>
    /// Interaction logic for RegisterPersonalPage.xaml
    /// </summary>
    public partial class RegisterPersonalPage : Page
    {

        public RegisterPersonalPage()
        {
            InitializeComponent();
        }

        private void capitalize_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            textBox.Text = Converter.CapitalizeWords(textBox.Text, 2);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbFirstName.Focus();
        }

        public string GetFirstName()
        {
            return tbFirstName.Text.Trim();
        }
        public string GetMiddleName()
        {
            string middleName = tbMiddleName.Text.Trim();
            if (string.IsNullOrEmpty(middleName))
            {
                return "N/A";
            }
            return middleName;
        }

        public string GetLastName()
        {
            return tbLastName.Text.Trim();
        }

        public string GetEmail()
        {
            return tbEmail.Text.Trim();
        }

        public string GetContact()
        {
            return tbContact.Text.Trim();
        }
    }
}
