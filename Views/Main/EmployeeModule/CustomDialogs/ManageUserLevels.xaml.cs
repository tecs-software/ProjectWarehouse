using System;
using System.Collections.Generic;
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

        public void RefreshTable()
        {

        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            NewUserLevel nul = new NewUserLevel();

            nul.Owner = Window.GetWindow(this);

            if (nul.ShowDialog() == true)
            {

            }
        }
    }
}
