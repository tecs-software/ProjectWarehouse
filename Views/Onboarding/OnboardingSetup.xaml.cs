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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views.Main;
using static WarehouseManagement.Models.Address;

namespace WarehouseManagement.Views.Onboarding
{
    /// <summary>
    /// Interaction logic for OnboardingSetup.xaml
    /// </summary>
    public partial class OnboardingSetup : Window
    {
        public OnboardingSetup()
        {
            InitializeComponent();
            load_couriers();
        }
        db_queries queries = new db_queries();
        private void load_couriers()
        {
            List<String> couriers = new List<String>();
            couriers.Add("JnT");

            cmbCourier.ItemsSource = couriers;
            queries.province(cmbProvince);
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            
            if(Util.IsAnyTextBoxEmpty(txtAddress,txtPagename,txtPhone) || Util.IsAnyComboBoxItemEmpty(cmbProvince) || Util.IsAnyComboBoxItemEmpty(cmbCity) || Util.IsAnyComboBoxItemEmpty(cmbBarangay))
            {
                MessageBox.Show("Please complete all required fields on sender information.");
                return;
            }
            else
            {
                if (Util.IsAnyTextBoxEmpty(txtCustomerID, txtEccompanyId, txtKey) || Util.IsAnyComboBoxItemEmpty(cmbCourier))
                {
                    MessageBox.Show("Please complete all required fields on customer information.");
                    return;
                }
                else
                {
                    if (queries.insert_sender(txtPagename, txtPhone, cmbProvince, cmbCity, cmbBarangay, txtAddress))
                    {
                        queries.api_credentials(cmbCourier, txtKey, txtEccompanyId, txtCustomerID);
                        MessageBox.Show("Information Setup completed");
                        MainWindow main = new MainWindow();
                        main.Show();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            this.Close();
        }
        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cmbCity.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.city(cmbCity, cmbProvince.Text);
        }

        private void cmbCity_DropDownClosed(object sender, EventArgs e)
        {
            cmbBarangay.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.baranggay(cmbBarangay, cmbCity.Text);
        }
    }
}
