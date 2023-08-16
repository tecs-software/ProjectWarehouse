﻿using Newtonsoft.Json;
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
using WWarehouseManagement.Database;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder
{
    public partial class ReceiverInformation : Page
    {
        private List<Address.Province>? provinces;
        private List<Address.Municipality>? municipalities;
        private List<Address.Barangay>? barangays;
        db_queries queries = new db_queries();
        sql_control sql = new sql_control();
        void CheckedRadio(string text)
        {
            if (text == "JNT")
            {
                JNTContainer.Visibility = Visibility.Visible;
                FlashContainer.Visibility = Visibility.Collapsed;

            }
            if (text == "FLASH")
            {
                JNTContainer.Visibility = Visibility.Collapsed;
                FlashContainer.Visibility = Visibility.Visible;
            }
        }
        public ReceiverInformation()
        {
            InitializeComponent();
            LoadAddress();
            JNTContainer.Visibility = Visibility.Collapsed;
            FlashContainer.Visibility = Visibility.Collapsed;
            lblBookingInfo.Visibility = Visibility.Collapsed;
            lblReceiverInfo.Visibility = Visibility.Visible;
            lblBookingInfo.Visibility = Visibility.Visible;
        }

        private async void LoadAddress()
        {
            //(provinces, municipalities, barangays) = await Util.LoadAddressData();

            //cbProvince.ItemsSource = provinces;
            //cbProvince.DisplayMemberPath = "province_name";
            queries.province(cbProvinceJnt);

        }

        private void cbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCityJnt.SelectedIndex = -1;
            cbBarangayJnt.ItemsSource = null;

            //if (cbProvince.SelectedItem is Address.Province selectedProvince && municipalities != null)
            //{
            //    List<Address.Municipality> filteredMunicipalities = municipalities.FindAll(m => m.province_id == selectedProvince.province_id);
            //    cbCity.ItemsSource = filteredMunicipalities;
            //    cbCity.DisplayMemberPath = "municipality_name";
            //}
        }

        private void cbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbBarangayJnt.SelectedIndex = -1;

            //if (cbCity.SelectedItem is Address.Municipality selectedMunicipality)
            //{
            //    List<Address.Barangay> filteredBarangays = barangays.FindAll(b => b.municipality_id == selectedMunicipality.municipality_id);
            //    cbBarangay.ItemsSource = filteredBarangays;
            //    cbBarangay.DisplayMemberPath = "barangay_name";
            //}
        }

        private void cbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cbCityJnt.SelectedIndex = -1;
            queries.city(cbCityJnt, cbProvinceJnt.Text);
        }

        private void cbCity_DropDownClosed(object sender, EventArgs e)
        {
            queries.baranggay(cbBarangayJnt, cbCityJnt.Text);
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

        private void tbAddress_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the entered text contains symbols
            if (HasSymbols(e.Text))
            {
                e.Handled = true; // Ignore the input
            }
        }
        private bool HasSymbols(string text)
        {
            foreach (char c in text)
            {
                // Check if the character is not alphanumeric, space, ",", ".", or "-"
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != ',' && c != '.' && c != '-')
                {
                    return true;
                }
            }
            return false;
        }
        private void cbItem_DropDownClosed(object sender, EventArgs e)
        {
            tbGoodsValue.Text = sql.ReturnResult($"SELECT nominated_price FROM tbl_products WHERE item_name = '{cbItem.Text}'");
        }

        private void tbQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            decimal total = Converter.StringToDecimal(tbGoodsValue.Text) * Converter.StringToDecimal(tbQuantity.Text);
            tbTotal.Text = Converter.StringToMoney(total.ToString());
        }
        private void rdbJandT_Checked(object sender, RoutedEventArgs e)
        {
            CheckedRadio(rdbJandT.Content.ToString());
        }

        private void rdbFlash_Checked(object sender, RoutedEventArgs e)
        {
            CheckedRadio(rdbFlash.Content.ToString());
        }
        
    }
}
