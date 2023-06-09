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

namespace WarehouseManagement.Views.InitialSetup
{
    /// <summary>
    /// Interaction logic for InitialSetupWindow.xaml
    /// </summary>
    public partial class InitialSetupWindow : Window
    {
        bool isServer = false, loading = false;
        string? connection;

        public InitialSetupWindow()
        {
            InitializeComponent();

            if (ConfigurationManager.ConnectionStrings["MyConnectionString"] != null)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }

            this.SizeToContent = SizeToContent.Height;
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

                }

                btnConnection.Content = "Check Connection";
                HideProgressBar();
            }
        }

        private async void btnProceed_Click(object sender, RoutedEventArgs e)
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
                        saveConnection(connection);
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

                }

                btnProceed.Content = "Proceed";
                HideProgressBar();
            }
            
        }

        private void saveConnection(string connectionString)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings("MyConnectionString", connectionString, "System.Data.SqlClient");
            config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
            config.Save(ConfigurationSaveMode.Modified);
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
        }

        private void Server()
        {
            Clear();
            isServer = true;
            tbAuthentication.Visibility = Visibility.Collapsed;
            tbPassword.Visibility = Visibility.Collapsed;
            tbServerName.Text = Util.GetLocalSqlServerInstanceName();
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
            progressBar.Visibility = Visibility.Visible;
        }

        private void HideProgressBar()
        {
            progressBar.Visibility = Visibility.Collapsed;
            loading = false;
            
        }
    }
}
