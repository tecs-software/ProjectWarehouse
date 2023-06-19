using System;
using System.Collections.Generic;
using System.Data;
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

namespace WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for ManageUserLevels.xaml
    /// </summary>
    public partial class ManageUserLevels : Window
    {
        public ManageUserLevels()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshTable();
        }

        public async void RefreshTable()
        {
            string query = @"
                SELECT r.role_id, r.role_name, r.hourly_rate, COUNT(DISTINCT al.user_id) AS user_count, COUNT(ma.module_name) AS permission_count
                FROM tbl_roles r
                LEFT JOIN tbl_access_level al ON r.role_id = al.role_id
                LEFT JOIN tbl_module_access ma ON r.role_id = ma.role_id
                GROUP BY r.role_id, r.role_name, r.hourly_rate
                ORDER BY permission_count DESC";

            DataTable? dataTable = await DBHelper.GetTable(query);

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);

                foreach (DataRowView row in dataView)
                {
                    row["role_name"] = Converter.CapitalizeWords(row["role_name"].ToString(), 2);
                }

                tblUserLevels.ItemsSource = dataView;
            }
            else
            {
                MessageBox.Show("Failed to retrieve user levels, database error.");
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            NewUserLevel nul = new NewUserLevel();

            nul.Owner = Window.GetWindow(this);

            if (nul.ShowDialog() == true)
            {
                RefreshTable();
            }

        }
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (tblUserLevels.SelectedItem == null)
                return;

            string? roleId = ((DataRowView)tblUserLevels.SelectedItem)["role_id"].ToString();

            NewUserLevel nul = new NewUserLevel(roleId);

            nul.Owner = Window.GetWindow(this);

            if (nul.ShowDialog() == true)
            {
                RefreshTable();
            }
        }

        

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
