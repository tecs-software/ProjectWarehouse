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
using NuGet;

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
        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if (txtMessageDialog.Text.Contains("has been released, do you want to update your app?"))
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
            int durationInSeconds = 3;
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

            try
            {
                using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                {
                    var updateInfo = await manager.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        var getFutureVersion = updateInfo.FutureReleaseEntry.Version;
                        string futureVersion = getFutureVersion.ToString();
                        CustomMessageBox("Version " + futureVersion + " has been released, do you want to update your app?", true);
                    }
                    else
                    {
                        string version = "Version " + updateInfo.CurrentlyInstalledVersion.Version.ToString();
                        GlobalModel.version = version;
                        new LoginWindow(GlobalModel.version).Show();
                        this.Close();

                    }
                }
            }
            catch
            {
                return;
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
