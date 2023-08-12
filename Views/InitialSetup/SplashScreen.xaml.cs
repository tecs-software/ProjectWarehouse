using Squirrel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using System.Windows.Controls.Primitives;
using WarehouseManagement.Views.Login;

namespace WarehouseManagement.Views.InitialSetup
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            StartLoopingProgressBar();
        }
        UpdateManager manager;
        private bool continueLoop = true;
        private bool CustomMessageBox(String message, Boolean questionType)
        {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            txtMessageDialog.Text = message;

            if (questionType)
            {
                btnYes.Content = "Yes";
                btnNo.Visibility = Visibility.Visible;
                dialog.IsOpen = true;
                return false;
            }
            else
            {
                btnYes.Content = "Okay";
                btnNo.Visibility = Visibility.Collapsed;
                dialog.IsOpen = true;
                return false;
            }
        }
        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if (txtMessageDialog.Text.Contains("New version released, you are about to update. Proceed?"))
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        await manager.UpdateApp();
                    }
                }

                MessageBox.Show("Update Successfully");
                await ClearConnection(); // Reset Server 
                // Restart application
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private async void btnNo_Click(object sender, RoutedEventArgs e)
        {
            GlobalModel.version = await getversion();
            new LoginWindow(await getversion()).Show();
            this.Close();
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
        private async void StartLoopingProgressBar()
        {
            while (continueLoop) // Check the flag in the loop condition
            {
                int durationInSeconds = 1;
                int steps = 1;
                int intervalMilliseconds = durationInSeconds * 1000 / steps;

                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 0,
                    To = 100,
                    Duration = TimeSpan.FromSeconds(durationInSeconds),
                    RepeatBehavior = new RepeatBehavior(1) // Repeat once
                };

                progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);

                await Task.Delay(intervalMilliseconds);
                progressBar.BeginAnimation(ProgressBar.ValueProperty, null); // Stop animation after 1 second

                bool shouldContinue = await CheckForUpdatesAndHandleMessage();
                if (!shouldContinue)
                {
                    continueLoop = false; // Set the flag to false to exit the loop
                }
            }
        }

        private async Task<bool> CheckForUpdatesAndHandleMessage()
        {
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        var getFutureVersion = updateInfo.FutureReleaseEntry.Version;
                        string futureVersion = getFutureVersion.ToString();
                        return CustomMessageBox("Version " + futureVersion + " has been released, do you want to update your app?", true);
                    }
                    else
                    {
                        GlobalModel.version = await getversion();
                        new LoginWindow(await getversion()).Show();
                        this.Close();
                        return false; // Don't continue the loop
                    }
                }
            }
            catch
            {
                return true; // Continue the loop
            }
        }
        private async Task<string> getversion()
        {
            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    string version = "Version " + updateInfo.CurrentlyInstalledVersion.Version.ToString();
                    return version;
                }
            }
            catch
            {
                return await getversion();
            }
        }
    }
}
