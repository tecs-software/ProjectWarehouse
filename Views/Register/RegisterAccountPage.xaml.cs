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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Views.Register
{
    /// <summary>
    /// Interaction logic for RegisterAccountPage.xaml
    /// </summary>
    public partial class RegisterAccountPage : Page
    {
        public RegisterAccountPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbUsername.Focus();
        }

        public string GetUserName()
        {
            return tbUsername.Text.Trim();
        }

        public string GetAuthentication()
        {
            return tbAuthentication.Text.Trim();
        }

        public SecureString GetPassword()
        {
            SecureString password = new SecureString();
            foreach (char c in tbPassword.Password)
            {
                password.AppendChar(c);
            }
            return password;
        }

        public SecureString GetConfirmPassword()
        {
            SecureString password = new SecureString();
            foreach (char c in tbConfirmPassword.Password)
            {
                password.AppendChar(c);
            }
            return password;
        }
    }
}
