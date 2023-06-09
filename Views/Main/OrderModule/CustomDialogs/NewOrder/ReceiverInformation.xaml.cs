using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder
{
    /// <summary>
    /// Interaction logic for ReceiverInformation.xaml
    /// </summary>
    public partial class ReceiverInformation : Page
    {
        private List<Address.Province> provinces;
        private List<Address.Municipality> municipalities;
        private List<Address.Barangay> barangays;
        public ReceiverInformation()
        {
            InitializeComponent();
            LoadAddress();
        }

        private async void LoadAddress()
        {
            await Task.Run(() =>
            {
                string provinceJson = File.ReadAllText("Components/table_province.json");
                provinces = JsonConvert.DeserializeObject<List<Address.Province>>(provinceJson);

                string municipalityJson = File.ReadAllText("Components/table_municipality.json");
                municipalities = JsonConvert.DeserializeObject<List<Address.Municipality>>(municipalityJson);

                string barangayJson = File.ReadAllText("Components/table_barangay.json");
                barangays = JsonConvert.DeserializeObject<List<Address.Barangay>>(barangayJson);

                provinces = provinces.OrderBy(p => p.province_name).ToList();
                municipalities = municipalities.OrderBy(m => m.municipality_name).ToList();
                barangays = barangays.OrderBy(b => b.barangay_name).ToList();
            });

            cbProvince.ItemsSource = provinces;
            cbProvince.DisplayMemberPath = "province_name";
        }

        private void cbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCity.SelectedIndex = -1; // Clear the selected item
            cbBarangay.ItemsSource = null; // Clear the items

            if (cbProvince.SelectedItem is Address.Province selectedProvince)
            {
                List<Address.Municipality> filteredMunicipalities = municipalities.FindAll(m => m.province_id == selectedProvince.province_id);
                cbCity.ItemsSource = filteredMunicipalities;
                cbCity.DisplayMemberPath = "municipality_name";
            }
        }

        private void cbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbBarangay.SelectedIndex = -1; // Clear the selected item

            if (cbCity.SelectedItem is Address.Municipality selectedMunicipality)
            {
                List<Address.Barangay> filteredBarangays = barangays.FindAll(b => b.municipality_id == selectedMunicipality.municipality_id);
                cbBarangay.ItemsSource = filteredBarangays;
                cbBarangay.DisplayMemberPath = "barangay_name";
            }
        }
    }
}
