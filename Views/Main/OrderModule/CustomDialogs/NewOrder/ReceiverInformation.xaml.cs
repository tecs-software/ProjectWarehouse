using MaterialDesignThemes.Wpf;
using Mono.Cecil.Cil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
        public void insert_item()
        {
            if(rdbJandT.IsChecked == true)
            {
                sql.Query($"SELECT sender_id FROM tbl_sender WHERE courier_id = 1");
                if (sql.HasException(true)) return;
                if(sql.DBDT.Rows.Count > 0)
                {
                    foreach(DataRow dr in sql.DBDT.Rows)
                    {
                        sql.Query($"SELECT * FROM tbl_products WHERE sender_id = '{dr[0].ToString()}'");
                        if (sql.HasException(true)) return;
                        if (sql.DBDT.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in sql.DBDT.Rows)
                            {
                                cbItem.Items.Add(dr1[2]);
                            }
                        }
                        //if (CurrentUser.Instance.userID == 1)
                        //{
                            
                        //}
                        //else
                        //{
                        //    int? sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_users WHERE user_id = {int.Parse(CurrentUser.Instance.userID.ToString())}"));
                        //    sql.Query($"SELECT * FROM tbl_products WHERE sender_id = {sender_id}");
                        //    if (sql.HasException(true)) return;
                        //    if (sql.DBDT.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow dr1 in sql.DBDT.Rows)
                        //        {
                        //            cbItem.Items.Add(dr1[2]);
                        //        }
                        //    }
                        //}
                    }
                }
            }
            else
            {
                sql.Query($"SELECT * FROM tbl_sender WHERE courier_id = 2");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        sql.Query($"SELECT * FROM tbl_products WHERE sender_id = '{dr[0].ToString()}'");
                        if (sql.HasException(true)) return;
                        if (sql.DBDT.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in sql.DBDT.Rows)
                            {
                                cbItem.Items.Add(dr1[2]);
                            }
                        }
                        //if (CurrentUser.Instance.userID == 1)
                        //{

                        //}
                        //else
                        //{
                        //    int? sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_users WHERE user_id = {int.Parse(CurrentUser.Instance.userID.ToString())}"));
                        //    sql.Query($"SELECT * FROM tbl_products WHERE sender_id = {sender_id}");
                        //    if (sql.HasException(true)) return;
                        //    if (sql.DBDT.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow dr1 in sql.DBDT.Rows)
                        //        {
                        //            cbItem.Items.Add(dr1[2]);
                        //        }
                        //    }
                        //}
                    }
                }
            }
        }

        void CheckedRadio(string text)
        {
            if (text == "J&T")
            {
                JNTContainer.Visibility = Visibility.Visible;
                FlashContainer.Visibility = Visibility.Collapsed;
                cbSizeFlash.Visibility = Visibility.Collapsed;
                cbItemType.Visibility = Visibility.Collapsed;
                cbOrderType.Visibility = Visibility.Collapsed;
                tbCod.Visibility = Visibility.Visible;
                HintAssist.SetHint(tbGoodsValue, "Total goods value (PHP)");
                tbQuantity.Visibility = Visibility.Visible;
                tbTotal.Visibility = Visibility.Visible;
                tbTotalHolder.Visibility = Visibility.Visible;
                tbTotalGoods.Visibility = Visibility.Visible;
                tbTotalGoodsHolder.Visibility = Visibility.Visible;
                queries.province(cbProvinceJnt);
            }
            if (text == "FLASH")
            {
                JNTContainer.Visibility = Visibility.Collapsed;
                FlashContainer.Visibility = Visibility.Visible;
                cbSizeFlash.Visibility = Visibility.Visible;
                cbItemType.Visibility = Visibility.Visible;
                cbOrderType.Visibility = Visibility.Visible;
                tbCod.Visibility = Visibility.Collapsed;
                HintAssist.SetHint(tbGoodsValue, "COD amount");
                tbQuantity.Visibility = Visibility.Collapsed;
                tbTotal.Visibility = Visibility.Collapsed;
                tbTotalHolder.Visibility = Visibility.Collapsed;
                tbTotalGoods.Visibility = Visibility.Collapsed;
                tbTotalGoodsHolder.Visibility = Visibility.Collapsed;
                queries.FlashProvince(cbProvinceFlash);
            }
        }
        void LoadTypes()
        {
            cbSizeFlash.Items.Clear();
            cbSizeFlash.Items.Add("MINI");
            cbSizeFlash.Items.Add("SMALL");
            cbSizeFlash.Items.Add("+SMALL");
            cbSizeFlash.Items.Add("MEDIUM");
            cbSizeFlash.Items.Add("+MEDIUM");
            cbSizeFlash.Items.Add("LARGE");

            cbOrderType.Items.Clear();
            cbOrderType.Items.Add("Cash on Delivery (COD)");
            cbOrderType.Items.Add("Non-Cash on Delivery (Non-COD)");

            cbItemType.Items.Clear();
            cbItemType.Items.Add("File");
            cbItemType.Items.Add("Dry food");
            cbItemType.Items.Add("Commodity");
            cbItemType.Items.Add("Digital products");
            cbItemType.Items.Add("Clothes");
            cbItemType.Items.Add("Books");
            cbItemType.Items.Add("Auto parts");
            cbItemType.Items.Add("Shoes and bags");
            cbItemType.Items.Add("Sports equipment");
            cbItemType.Items.Add("Cosmetics");
            cbItemType.Items.Add("Household");
            cbItemType.Items.Add("Fruit");
            cbItemType.Items.Add("Others");
        }
        public ReceiverInformation()
        {
            InitializeComponent();
            LoadTypes();
            JNTContainer.Visibility = Visibility.Collapsed;
            FlashContainer.Visibility = Visibility.Collapsed;
            lblBookingInfo.Visibility = Visibility.Collapsed;
            cbSizeFlash.Visibility = Visibility.Collapsed;
            lblReceiverInfo.Visibility = Visibility.Visible;
            lblBookingInfo.Visibility = Visibility.Visible;
            rdbJandT.IsChecked = true;
            rdbFlash.IsChecked = false;
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
            if (rdbJandT.IsChecked == true)
            {
                sql.AddParam("@item", cbItem.Text);
                tbGoodsValue.Text = sql.ReturnResult($"SELECT nominated_price FROM tbl_products WHERE item_name = @item");

                if (tbQuantity.Text != "")
                {
                    decimal total = (Converter.StringToDecimal(tbGoodsValue.Text) * Converter.StringToDecimal(tbQuantity.Text)) + Converter.StringToDecimal(tbCod.Text);
                    tbTotal.Text = Converter.StringToMoney(total.ToString());
                }
            }
            else
            {
                //do nothing
            }
        }
        private void tbQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            decimal total = (Converter.StringToDecimal(tbGoodsValue.Text) * Converter.StringToDecimal(tbQuantity.Text));
            tbTotalGoods.Text = total.ToString();
        }
        private void rdbJandT_Checked(object sender, RoutedEventArgs e)
        {
            cbItem.Items.Clear();
            insert_item();
            CheckedRadio(rdbJandT.Content.ToString());
            sql.AddParam("@item", cbItem.Text);
            tbGoodsValue.Text = sql.ReturnResult($"SELECT nominated_price FROM tbl_products WHERE item_name = @item");

            if (tbQuantity.Text != "")
            {
                decimal total = (Converter.StringToDecimal(tbGoodsValue.Text) * Converter.StringToDecimal(tbQuantity.Text)) + Converter.StringToDecimal(tbCod.Text);
                tbTotal.Text = Converter.StringToMoney(total.ToString());
            }
        }

        private void rdbFlash_Checked(object sender, RoutedEventArgs e)
        {
            cbItem.Items.Clear();
            insert_item();
            CheckedRadio(rdbFlash.Content.ToString());
            tbGoodsValue.Text = "";
        }

        private void cbProvinceFlash_DropDownClosed(object sender, EventArgs e)
        {
            cbCityFlash.SelectedIndex = -1;
            queries.FlashCity(cbCityFlash, cbProvinceFlash.Text);
        }

        private void cbProvinceFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbCityFlash.SelectedIndex = -1;
            cbBarangayFlash.ItemsSource = null;
        }

        private void cbCityFlash_DropDownClosed(object sender, EventArgs e)
        {
            queries.FlashBaranggay(cbBarangayFlash, cbCityFlash.Text);
        }

        private void cbCityFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbBarangayFlash.SelectedIndex = -1;
        }

        private void cbBarangayFlash_DropDownClosed(object sender, EventArgs e)
        {
            queries.FlashPostalCode(cbPostalCodeFlash, cbBarangayFlash.Text);
        }

        private void cbBarangayFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbPostalCodeFlash.SelectedIndex = -1;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tbCod_KeyUp(object sender, KeyEventArgs e)
        {
            tbTotal.Text = "₱" + Converter.StringToMoney(tbCod.Text);
        }
    }
}
