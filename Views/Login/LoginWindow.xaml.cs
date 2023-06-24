using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WarehouseManagement.Models;
using WarehouseManagement.Views.InitialSetup;
using WarehouseManagement.Views.Main;
using WarehouseManagement.Views.Onboarding;
using WarehouseManagement.Views.Register;

namespace WarehouseManagement.Views.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        bool loading;

        public LoginWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;
        }

        db_queries queries = new db_queries();
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!loading)
            {
                ShowLoading();

                if (Util.IsAnyTextBoxEmpty(tbUsername, tbPassword))
                {
                    ShowMessage("Invalid Credentials");
                    HideLoading();
                    return;
                }

                string username = tbUsername.Text;
                string password = Util.HashPassword(tbPassword.SecurePassword);

                DBHelper db = new();

                if (!await db.IsDataExistsAsync("tbl_users", "username", username))
                {
                    ShowMessage("User does not exist!");
                    HideLoading();
                    return;
                }

                if (await db.AuthenticateUser(username, password))
                {
                    if(queries.check_sender_info())
                    {
                        
                    }
                    else
                    {

                        MainWindow main = new MainWindow();
                        main.Show();
                        //OnboardingSetup onboarding = new OnboardingSetup();
                        //onboarding.Show();
                    }
                    
                    
                    this.Close();
                }
                else
                {
                    ShowMessage("Invalid Credentials");
                }

                HideLoading();
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            HideMessage();
        }

        private void tbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            HideMessage();
        }

        private void ShowLoading()
        {
            loading = true;
            progressBar.Visibility = Visibility.Visible;
        }

        private void HideLoading()
        {
            progressBar.Visibility = Visibility.Collapsed;
            loading = false;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Foreground = Brushes.Red;
            lblMessage.Visibility = Visibility.Visible;
            lblMessage.Content = message;
        }

        private void HideMessage()
        {
            lblMessage.Visibility = Visibility.Collapsed;
        }

        private async Task ClearConnection()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings connectionStringSettings = config.ConnectionStrings.ConnectionStrings["MyConnectionString"];
            if (connectionStringSettings != null)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(connectionStringSettings);
                await Task.Run(() => config.Save(ConfigurationSaveMode.Modified));
            }
        }

        private void btnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();
            register.Show();
            this.Close();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the connection? You'll need to restart the app.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ClearConnection();
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Mouse.Captured == null)
                {
                    DragMove();
                }
            }
        }
    }
}
