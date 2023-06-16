using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.InventoryModule;
using MenuItem = WarehouseManagement.Models.MenuItem;

namespace WarehouseManagement.Views.Main.EmployeeModule
{
    /// <summary>
    /// Interaction logic for EmployeeMenu.xaml
    /// </summary>
    public partial class EmployeeMenu : UserControl
    {
        public EmployeeMenu(MenuItem itemMenu)
        {
            InitializeComponent();
            ExpanderMenu.Visibility = itemMenu.SubMenuItems == null ? Visibility.Collapsed : Visibility.Visible;
            ListViewItemMenu.Visibility = itemMenu.SubMenuItems == null ? Visibility.Visible : Visibility.Collapsed;

            DataContext = itemMenu;
        }

        private object? _previousSelectedItem;

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ListViewMenu.SelectedItem;


            //MessageBox.Show(selectedItem.ToString());

            if (selectedItem != null)
            {
                var header = ((SubMenuItem)selectedItem).Name;

                var parent = VisualTreeHelper.GetParent(this);



                while (parent is not EmployeeView)
                {

                    parent = VisualTreeHelper.GetParent(parent);

                }

                var employee = parent as EmployeeView;



                if (header != null)
                {
                    switch (header.ToLower())
                    {
                        case "active":

                            employee?.ActiveEmployees();
                            break;
                        case "offline":

                            employee?.InactiveEmployees();

                            break;
                        case "archived":

                            employee?.DisabledEmployees();
                            break;

                    }
                }

                // Deselect the previously selected item
                if (_previousSelectedItem != null && _previousSelectedItem != selectedItem)
                {
                    ListViewMenu.SelectedItem = null;
                    _previousSelectedItem = null;
                }
                else
                {
                    _previousSelectedItem = selectedItem;

                }
            }
        }

        private void ExpanderMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string? header = ExpanderMenu.Header.ToString();

            var parent = VisualTreeHelper.GetParent(this);


            while (parent is not EmployeeView)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var employee = parent as EmployeeView;

            if (header != null)
            {
                switch (header.ToLower())
                {
                    case "people":
                        ListViewMenu.SelectedItem = null;
                        employee?.Employee();
                        break;
                    case "run payroll":

                        employee?.Payroll();

                        break;
                }

            }


        }
    
    }
}
