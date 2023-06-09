﻿using System;
using System.Collections.Generic;
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
using WarehouseManagement.Views.Main;

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

                if (!await DatabaseHelper.IsDataExistsAsync("tbl_users", "username", username))
                {
                    ShowMessage("User does not exist!");
                    HideLoading();
                    return;
                }

                if (await DatabaseHelper.AuthenticateUser(username, password))
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
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
    }
}
