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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder
{
    public partial class ReceiverInformation : Page
    {
        private List<Address.Province>? provinces;
        private List<Address.Municipality>? municipalities;
        private List<Address.Barangay>? barangays;
        db_queries queries = new db_queries();

        public ReceiverInformation()
        {
            InitializeComponent();
            LoadAddress();
        }

        private async void LoadAddress()
        {
            //(provinces, municipalities, barangays) = await Util.LoadAddressData();

            //cbProvince.ItemsSource = provinces;
            //cbProvince.DisplayMemberPath = "province_name";
            queries.province(cbProvince);

        }

        private void cbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCity.SelectedIndex = -1;
            cbBarangay.ItemsSource = null;

            //if (cbProvince.SelectedItem is Address.Province selectedProvince && municipalities != null)
            //{
            //    List<Address.Municipality> filteredMunicipalities = municipalities.FindAll(m => m.province_id == selectedProvince.province_id);
            //    cbCity.ItemsSource = filteredMunicipalities;
            //    cbCity.DisplayMemberPath = "municipality_name";
            //}
        }

        private void cbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbBarangay.SelectedIndex = -1;

            //if (cbCity.SelectedItem is Address.Municipality selectedMunicipality)
            //{
            //    List<Address.Barangay> filteredBarangays = barangays.FindAll(b => b.municipality_id == selectedMunicipality.municipality_id);
            //    cbBarangay.ItemsSource = filteredBarangays;
            //    cbBarangay.DisplayMemberPath = "barangay_name";
            //}
        }

        private void cbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cbCity.SelectedIndex = -1;
            queries.city(cbCity, cbProvince.Text);
        }

        private void cbCity_DropDownClosed(object sender, EventArgs e)
        {
            queries.baranggay(cbBarangay,cbCity.Text);
        }

        private void capitalize_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            textBox.Text = Converter.CapitalizeWords(textBox.Text, 2);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }
    }
}
