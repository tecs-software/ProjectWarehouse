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
using System.IO;
using MaterialDesignThemes.Wpf;

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
            getSplashScreenImage();
            checkForUpdates();
            Trial_Controller.updateModules();
            Trial_Controller.InsertTrialDay();
        }
        UpdateManager manager;
         void CustomMessageBox(String message, Boolean questionType)
         {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            Key.Visibility = Visibility.Collapsed;
            txtMessageDialog.Text = message;

            if (questionType)
            {
                btnYes.Content = "Update";
                btnNo.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnNo.Content = "Proceed";
                btnYes.Visibility= Visibility.Collapsed;
                Key.Visibility = Visibility.Visible;
            }
            dialog.IsOpen = true;
        }
        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            startProgressBar();
            if (txtMessageDialog.Text.Contains("has been released, update your app now!"))
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
            if (txtMessageDialog.Text.Contains("Subscription is expired. Please contact your distributor of the application."))
            {
                if (Key.Password == "142977")
                {
                    Trial_Controller.refreshSubs();
                    new LoginWindow(GlobalModel.version).Show();
                    this.Close();
                }
                else
                {
                    Key.Password = "";
                    await Task.Delay(1000);
                    if (Trial_Controller.IsTrialEnded())
                    {
                        CustomMessageBox("Subscription is expired. Please contact your distributor of the application.", false);
                    }
                }
            }
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
        private async void startProgressBar()
        {
            int durationInSeconds = 7;
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
        }
        private async void checkForUpdates()
        {
            int durationInSeconds = 2;
            int steps = 1;
            int intervalMilliseconds = durationInSeconds * 1000 / steps;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromSeconds(durationInSeconds),
                RepeatBehavior = new RepeatBehavior(1) // Repeat once
            };

            bool connected = false;
            while (!connected)
            {
                try
                {
                    // Start the progress bar animation
                    progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
                    using (var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse"))
                    {
                        var updateInfo = await manager.CheckForUpdate();
                        if (updateInfo.ReleasesToApply.Count > 0)
                        {
                            var getFutureVersion = updateInfo.FutureReleaseEntry.Version;
                            string futureVersion = getFutureVersion.ToString();
                            CustomMessageBox("Version " + futureVersion + " has been released, update your app now!", true);
                        }
                        else
                        {
                            string version = "Version " + updateInfo.CurrentlyInstalledVersion.Version.ToString();
                            GlobalModel.version = version;
                            if (Trial_Controller.IsTrialEnded())
                            {
                                CustomMessageBox("Subscription is expired. Please contact your distributor of the application.", false);
                            }
                            else
                            {
                                new LoginWindow(GlobalModel.version).Show();
                                this.Close();
                            }
                        }
                        // If the code reached here, the connection was successful
                        connected = true;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions and display an error message
                    MessageBox.Show(ex.Message + ". Please click (OK) and it will try again to check for updates or get your application's current version.");

                    // Stop the progress bar animation
                    progressBar.BeginAnimation(ProgressBar.ValueProperty, null);

                    // Delay before retrying
                    await Task.Delay(2500); // 2.5 seconds delay between retries
                }
            }
        }
       private void getSplashScreenImage()
       {
            string defaultImageFileName = "TecsLogo.png"; // Replace with the actual default image file name

            // Construct the path to the default image file
            string componentsDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Components");
            string defaultImagePath = System.IO.Path.Combine(componentsDirectory, defaultImageFileName);

            // Check if the default image file exists
            if (File.Exists(defaultImagePath))
            {
                // Create a BitmapImage and set it as the source of imgSplashScreen
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(defaultImagePath, UriKind.Absolute);
                bitmapImage.EndInit();
                imgSplashScreen.Source = bitmapImage;
            }
            else
            {
                // Default image file does not exist
                MessageBox.Show("TECS logo not detected.");
            }

       }
        private void trials()
        {
            CustomMessageBox("Subscription is expired. Please contact your distributor of the application.", false);
            //while (true)
            //{
            //    if (Trial_Controller.IsTrialEnded())
            //    {
            //        //MessageBox.Show("Subscription is expired. Please contact your distributor of the application.");

            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
