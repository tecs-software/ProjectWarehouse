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
    /// Interaction logic for ViewGeneratedKeys.xaml
    /// </summary>
    public partial class ViewGeneratedKeys : Window
    {
        public ViewGeneratedKeys()
        {
            InitializeComponent();
            RefreshTable();
        }

        public async void RefreshTable()
        {
            string query = @"
                SELECT u.user_id, u.username, u.authentication_code, al.role_id, r.role_name,
                       CASE WHEN u.username IS NOT NULL THEN 'Used' ELSE 'Not Used' END AS status
                FROM tbl_users u
                LEFT JOIN tbl_access_level al ON u.user_id = al.user_id
                LEFT JOIN tbl_roles r ON al.role_id = r.role_id";

            DataTable? dataTable = await DBHelper.GetTable(query);

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                tblKeys.ItemsSource = dataView;
            }
            else
            {
                MessageBox.Show("Failed to retrieve user levels, database error.");
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (tblKeys.SelectedItem == null)
                return;

            string? code = ((DataRowView)tblKeys.SelectedItem)["authentication_code"].ToString();
            Clipboard.SetText(code);
            MessageBox.Show("Authentication code copied to clipboard", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
