using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Login;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;

namespace WarehouseManagement.Views.Register
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private RegisterPersonalPage registerPersonalPage;
        private RegisterAccountPage registerAccountPage;
        User newUser;

        public RegisterWindow()
        {
            InitializeComponent();
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(GetOrCreateRegisterPersonalPage());
            newUser = new User();
            this.SizeToContent = SizeToContent.Height;
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame.Content is RegisterPersonalPage)
            {
                RegisterPersonalPage? personalPage = mainFrame.Content as RegisterPersonalPage;

                if(personalPage == null)
                {
                    return;
                }

                if(Util.IsAnyStringEmpty(personalPage.GetFirstName(), personalPage.GetLastName(), personalPage.GetLastName(), personalPage.GetEmail(), personalPage.GetContact()))
                {
                    MessageBox.Show("Do not leave any field blank");
                    return;
                }

                newUser.firstName = personalPage.GetFirstName();
                newUser.middleName = personalPage.GetMiddleName();
                newUser.lastName = personalPage.GetLastName();
                newUser.email = personalPage.GetEmail();
                newUser.contactNumber = personalPage.GetContact();

                mainFrame.Navigate(GetOrCreateRegisterAccountPage());
                lblTitle.Text = "Register | Account Information";
                btnNext.Content = "Complete";
                btnPrev.Text = "Back";
            }
            else if (mainFrame.Content is RegisterAccountPage)
            {
                DBHelper db = new DBHelper();

                RegisterAccountPage? accountPage = mainFrame.Content as RegisterAccountPage;

                if (accountPage == null)
                {
                    return;
                }

                if (Util.IsAnyStringEmpty(accountPage.GetUserName(), accountPage.GetPassword(), accountPage.GetConfirmPassword(), accountPage.GetAuthentication()))
                {
                    MessageBox.Show("Do not leave any field blank");
                    return;
                }

                if (await db.IsDataExistsAsync("tbl_users", "username", accountPage.GetUserName()))
                {
                    MessageBox.Show("Username already exists!");
                    return;
                }


                if (accountPage.GetPassword().Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters long.");
                    return;
                }

                if (!Util.SecurePasswordsMatch(accountPage.GetPassword(), accountPage.GetConfirmPassword()))
                {
                    MessageBox.Show("Password do not match");
                    return;
                }

                if (!await db.AuthenticationCheck("tbl_users", "authentication_code", accountPage.GetAuthentication()))
                {
                    MessageBox.Show("Failed to authenticate. Please check your login credentials1.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                newUser.username = accountPage.GetUserName();
                newUser.password = accountPage.GetPassword();
                newUser.authenticationCode = accountPage.GetAuthentication();

                if (await db.RegisterUser("tbl_users", new string[] { "first_name", "middle_name", "last_name", "email", "contact_number", "username", "password" },
                                                            new string[] { newUser.firstName, newUser.middleName, newUser.lastName, newUser.email, newUser.contactNumber, newUser.username, Util.HashPassword(newUser.password)},
                                                            "authentication_code", newUser.authenticationCode))
                {
                    MessageBox.Show("You have successfully registered! You may now proceed to login");

                    LoginWindow login = new LoginWindow(GlobalModel.version);
                    login.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to authenticate. Please check your login credentials2.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void btnPrev_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mainFrame.CanGoBack)
            {
                if (mainFrame.Content is RegisterAccountPage)
                {
                    lblTitle.Text = "Register | Personal Information";
                    btnNext.Content = "Next";
                    btnPrev.Text = "Back to Login";
                }

                mainFrame.GoBack();
            }
            else
            {
                LoginWindow login = new LoginWindow(GlobalModel.version);
                login.Show();
                this.Close();
            }
        }

        private RegisterPersonalPage GetOrCreateRegisterPersonalPage()
        {
            if (registerPersonalPage == null)
            {
                registerPersonalPage = new RegisterPersonalPage();
            }

            return registerPersonalPage;
        }

        private RegisterAccountPage GetOrCreateRegisterAccountPage()
        {
            if (registerAccountPage == null)
            {
                registerAccountPage = new RegisterAccountPage();
            }

            return registerAccountPage;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
