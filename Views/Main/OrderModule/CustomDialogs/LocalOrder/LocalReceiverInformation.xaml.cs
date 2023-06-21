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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.LocalOrder
{
    /// <summary>
    /// Interaction logic for LocalReceiverInformation.xaml
    /// </summary>
    public partial class LocalReceiverInformation : Page
    {
        private List<Address.Province>? provinces;
        private List<Address.Municipality>? municipalities;
        private List<Address.Barangay>? barangays;

        public LocalReceiverInformation()
        {
            InitializeComponent();
            LoadAddress();
        }

        private async void LoadAddress()
        {
            (provinces, municipalities, barangays) = await Util.LoadAddressData();

            cbProvince.ItemsSource = provinces;
            cbProvince.DisplayMemberPath = "province_name";
        }

        private void cbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbBarangay.SelectedIndex = -1;

            if (cbCity.SelectedItem is Address.Municipality selectedMunicipality)
            {
                List<Address.Barangay> filteredBarangays = barangays.FindAll(b => b.municipality_id == selectedMunicipality.municipality_id);
                cbBarangay.ItemsSource = filteredBarangays;
                cbBarangay.DisplayMemberPath = "barangay_name";
            }
        }

        private void cbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCity.SelectedIndex = -1;
            cbBarangay.ItemsSource = null;

            if (cbProvince.SelectedItem is Address.Province selectedProvince && municipalities != null)
            {
                List<Address.Municipality> filteredMunicipalities = municipalities.FindAll(m => m.province_id == selectedProvince.province_id);
                cbCity.ItemsSource = filteredMunicipalities;
                cbCity.DisplayMemberPath = "municipality_name";
            }
            
        }

        private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
