using Squirrel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using WarehouseManagement.Views.InitialSetup;
using WarehouseManagement.Views.Main;
using WarehouseManagement.Views.Main.SystemSettingModule;
using WarehouseManagement.Views.Onboarding;
using WarehouseManagement.Views.Register;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Views.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region
        [DllImport("TrialApp.dll", EntryPoint = "ReadSettingsStr", CharSet = CharSet.Ansi)]
        static extern uint InitTrial(String aKeyCode, IntPtr aHWnd);
        [DllImport("TrialApp.dll", EntryPoint = "DisplayRegistrationStr", CharSet = CharSet.Ansi)]
        static extern uint DisplayRegistration(String aKeyCode, IntPtr aHWnd);

        // The kLibraryKey is meant to prevent unauthorized use of the library.
        // Do not share this key. Replace this key with your own from Advanced Installer 
        // project > Licensing > Registration > Library Key
        private const string kLibraryKey = "9F14215F9F14FC2E9A7E567D41D69FB6C2DCF1E938A40F73912EEBC3FF32F16FCA4BC36F897C";

        private static void OnInit()
        {
            try
            {
                Process process = Process.GetCurrentProcess();
                InitTrial(kLibraryKey, process.MainWindowHandle);
            }
            catch (DllNotFoundException ex)
            {
                // Trial dll is missing close the application immediately.
                MessageBox.Show(ex.ToString());
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.ToString());
            }
        }

        private void RegisterApp(object sender, EventArgs e)
        {
            try
            {
                Process process = Process.GetCurrentProcess();
                DisplayRegistration(kLibraryKey, process.MainWindowHandle);
            }
            catch (DllNotFoundException ex)
            {
                // Trial dll is missing close the application immediately.
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.ToString());
            }
        }

        #endregion
        bool loading;
        void CustomMessageBox(String message, Boolean questionType)
        {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            txtMessageDialog.Text = message;
            if (questionType)
            {
                btnYes.Content = "Yes";
                btnNo.Visibility = Visibility.Visible;
            }
            else
            {
                btnYes.Content = "Okay";
                btnNo.Visibility = Visibility.Collapsed;
            }
            dialog.IsOpen = true;
        }

        public LoginWindow()
        {
            InitializeComponent();
            
            tbUsername.Focus();
            this.SizeToContent = SizeToContent.Height;
        }
        private async void getversion()
        {
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    lbl_Versions.Text = "Version " + updateInfo.CurrentlyInstalledVersion.Version.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        db_queries queries = new db_queries();
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Trial_Controller.IsTrialEnded())
            {
                MessageBox.Show("Trial is expired. Please contact your distributor of the application.");
                return;
            }
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

                    if (queries.check_sender_info())
                    {
                        MainWindow main = new MainWindow();
                        main.Show();
                    }
                    else
                    {
                        OnboardingSetup onboarding = new OnboardingSetup();
                        onboarding.Show();
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
        UpdateManager manager;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Trial_Controller.InsertTrialDay();
            //checking for patch
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        var getFutureVersion = updateInfo.FutureReleaseEntry.Version;
                        string futureVersion = getFutureVersion.ToString();
                        CustomMessageBox(futureVersion + " New version released, you are about to update. Proceed?", true);
                    }
                    else
                    {
                        getversion();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if(txtMessageDialog.Text.Contains("New version released, you are about to update. Proceed?"))
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        await manager.UpdateApp();
                    }
                }

                MessageBox.Show("Update Succesfully");
                await ClearConnection(); // Reset Server 
                // Restart application
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
