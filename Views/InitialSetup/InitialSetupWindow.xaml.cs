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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views.Login;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Views.InitialSetup
{
    /// <summary>
    /// Interaction logic for InitialSetupWindow.xaml
    /// </summary>
    public partial class InitialSetupWindow : Window
    {
        bool isServer = false, loading = false;

        string? connection;
        UpdateManager manager;
 
        public InitialSetupWindow()
        {
            InitializeComponent();
            if (ConfigurationManager.ConnectionStrings["MyConnectionString"] != null)
            {
                //Loaded += Window_Loaded;
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/bengbeng09/ProjectWarehouse");

            //var updateInfo = await manager.CheckForUpdate();
            //if (updateInfo.ReleasesToApply.Count > 0)
            //{
            //    await manager.UpdateApp();  //Download Update
            //    MessageBox.Show("Updated Application");
            //    //Reset Server 
            //    //Restart application
            //}
            //else
            //{
            //    LoginWindow login = new LoginWindow();
            //    login.Show();
            //    this.Close();
            //}
            //ReCenter();
        }


        private async void btnConnection_Click(object sender, RoutedEventArgs e)
        {
            if (!loading)
            {
                HideMessage();
                ShowProgressBar();
                btnConnection.Content = "Checking Connection....";
 
                if (isServer)
                {
                    if (Util.IsAnyTextBoxEmpty(tbServerName))
                    {
                        return;
                    }

                    connection = $"Data Source={tbServerName.Text};Integrated Security=True";


                    if (await DatabaseConnection.TestConnection(connection))
                    {
                        btnProceed.Visibility = Visibility.Visible;
                        ShowMessage("Connection Success");
                    }
                    else
                    {
                        ShowMessage("Connection Failed");
                    }
                }
                else
                {
                    if (Util.IsAnyTextBoxEmpty(tbServerName, tbAuthentication, tbPassword))
                    {
                        return;
                    }

                    connection = $"Data Source={tbServerName.Text};Initial Catalog=db_warehouse_management;User ID={tbAuthentication.Text};Password={tbPassword.Text};Integrated Security=False";
                    if (await DatabaseConnection.TestConnection(connection))
                    {
                        btnProceed.Visibility = Visibility.Visible;
                        ShowMessage("Connection Success");
                    }
                    else
                    {
                        ShowMessage("Connection Failed");
                    }
                }

                btnConnection.Content = "Check Connection";
                HideProgressBar();
            }
        }

        private async void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!loading)
                {
                    ShowProgressBar();
                    btnProceed.Content = "Setting Up, Please Wait.... ";

                    if (isServer && !string.IsNullOrWhiteSpace(connection))
                    {
                        DatabaseInitializer dbInitializer = new DatabaseInitializer(connection);

                        if (await dbInitializer.CreateDatabaseIfNotExists("db_warehouse_management"))
                        {

                            connection += ";Initial Catalog=db_warehouse_management";
                            await SaveConnection(connection);
                            sql_control sql = new sql_control();
                            sql.Query("INSERT INTO tbl_trial_key(Product_Key) VALUES ('N9TT-9G0A-B7FQ-RANC')");
                            await dbInitializer.InsertSQLAuthentication("db_warehouse_management", connection);
                            LoginWindow login = new LoginWindow();
                            login.Show();
                            this.Close();

                        }
                        else
                        {
                            ShowMessage("Setting up failed, please try again.");

                        }
                    }
                    else
                    {
                        await SaveConnection(connection);
                        LoginWindow login = new LoginWindow();
                        login.Show();
                        this.Close();
                    }

                    btnProceed.Content = "Proceed";
                    HideProgressBar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async Task SaveConnection(string connectionString)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings("MyConnectionString", connectionString, "System.Data.SqlClient");
            config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
            await Task.Run(() => config.Save(ConfigurationSaveMode.Modified));
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            HideMessage();
            btnProceed.Visibility = Visibility.Collapsed;
            btnProceed.Visibility = Visibility.Collapsed;
        }

        private void rbClient_Checked(object sender, RoutedEventArgs e)
        {
            Client();
        }

        private void rbServer_Checked(object sender, RoutedEventArgs e)
        {
            Server();
        }

        private void Client()
        {
            Clear();
            isServer = false;
            tbAuthentication.Visibility = Visibility.Visible;
            tbPassword.Visibility = Visibility.Visible;
            ReCenter();
        }

        private void Server()
        {
            Clear();
            isServer = true;
            tbAuthentication.Visibility = Visibility.Collapsed;
            tbPassword.Visibility = Visibility.Collapsed;
            tbServerName.Text = Util.GetLocalSqlServerInstanceName();
            ReCenter();

        }

        private void Clear()
        {
            HideMessage();
            btnProceed.Visibility = Visibility.Collapsed;
            layoutConnection.Visibility = Visibility.Visible;
            tbAuthentication.Clear();
            tbPassword.Clear();
            tbServerName.Clear();
        }

        private void ShowMessage(string message)
        {
            if (message.ToLower().Contains("success"))
            {
                lblMessage.Foreground = new SolidColorBrush(Color.FromRgb(4, 181, 173));
            }
            else
            {
                lblMessage.Foreground = Brushes.Red;
            }

            lblMessage.Visibility = Visibility.Visible;
            lblMessage.Content = message;
        }

        private void HideMessage()
        {
            
            lblMessage.Visibility = Visibility.Collapsed;
        }

        private void ShowProgressBar()
        {
            loading = true;
            rbClient.IsEnabled = false;
            rbServer.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;
        }

        private void HideProgressBar()
        {
            progressBar.Visibility = Visibility.Collapsed;
            rbClient.IsEnabled = true;
            rbServer.IsEnabled = true;
            loading = false;
            
        }

       

        private void ReCenter()
        {
            this.SizeToContent = SizeToContent.Height;

            this.Loaded += (sender, e) =>
            {
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double windowWidth = ActualWidth;
                double windowHeight = ActualHeight;

                Left = (screenWidth - windowWidth) / 2;
                Top = (screenHeight - windowHeight) / 2;
            };
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
