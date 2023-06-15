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
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.InventoryModule.CustomDialogs
{
    public partial class ProfitAnalysis : Window
    {
        public Product? product;
        public int mode = 0;

        public ProfitAnalysis(Product? product)
        {
            InitializeComponent();
            this.product = product;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mode == 1 && product != null)
            {
                cbItems.Items.Add(product.ItemName);
                cbItems.SelectedItem = product.ItemName;
                cbItems.IsEnabled = false;
                cbItems.IsEditable = false;
                tbCostOfGood.Text = product.AcqCost.ToString();
                tbNominated.Text = product.NominatedPrice.ToString();
                tbQuantity.Text = product.UnitQuantity.ToString();
            }
            else if (mode == 2)
            {
                DBHelper db = new DBHelper();

                IEnumerable<string> columnNames = new List<string>()
                {
                    "product_id",
                    "item_name",
                    "acq_cost",
                    "unit_quantity",
                    "nominated_price"
                };

                Dictionary<string, object> excludedFilters = new Dictionary<string, object>()
                {
                    { "status", "discontinued" },
                };

                List<Dictionary<string, object>>? rows = await db.GetRowsExcluded("tbl_products", columnNames, excludedFilters);

                List<Product> products = new List<Product>();

                if (rows != null)
                {
                    foreach (Dictionary<string, object> row in rows)
                    {
                        Product product = new Product();
                        product.ProductId = row["product_id"].ToString();
                        product.ItemName = row["item_name"].ToString();
                        product.NominatedPrice = Convert.ToDecimal(row["nominated_price"]);
                        product.AcqCost = Convert.ToDecimal(row["acq_cost"]);
                        product.UnitQuantity = Convert.ToInt32(row["unit_quantity"]);
                        products.Add(product);
                    }

                }

                if (products != null)
                {
                    Product noneProduct = new Product();
                    noneProduct.ItemName = "None";
                    products.Insert(0, noneProduct);
                    cbItems.ItemsSource = products;
                    cbItems.DisplayMemberPath = "ItemName";
                }

            }
        }

        private async void cbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Util.ClearTextBoxes(tbCostOfGood, tbFactor, tbNominated, tbAdsBudget, tbROAS, tbAdsSpent, tbPlatform, tbEmployeeCommission,
                       tbShippingFee, tbRtsMargin, tbSellingTotal, tbTotalCost, tbNetProfit, tbNetProfitPerMonth);

            if ((cbItems.SelectedItem != null || cbItems.Text != "Choose Item") && mode == 2 && ((Product)cbItems.SelectedItem).ItemName != "None")
            {
                btnUpdate.Visibility = Visibility.Visible;
                decimal? cost = ((Product)cbItems.SelectedItem).AcqCost;
                decimal? nominatedPrice = ((Product)cbItems.SelectedItem).NominatedPrice;
                int? unitQuantity = ((Product)cbItems.SelectedItem).UnitQuantity;

                tbQuantity.Text = unitQuantity.ToString();
                tbCostOfGood.Text = cost?.ToString("N2");
                tbNominated.Text = nominatedPrice?.ToString("N2");

                DBHelper db = new DBHelper();

                IEnumerable<string> columnNames = new List<string>()
                {
                    "ads_budget",
                    "roas",
                    "adspent_per_item",
                    "platform_commission",
                    "employee_commission",
                    "shipping_fee",
                    "rts_margin"
                };

                Dictionary<string, object>? row = await db.GetRow("tbl_selling_expenses", columnNames, "product_id", ((Product)cbItems.SelectedItem).ProductId);

                if (row != null && row.Count > 0)
                {
                    tbAdsBudget.Text =  Converter.StringToMoney(row["ads_budget"].ToString()).ToString();
                    tbROAS.Text = Converter.StringToInteger(row["roas"].ToString()).ToString();
                    tbPlatform.Text = Converter.StringToPercentage(row["platform_commission"].ToString()).ToString();
                    tbEmployeeCommission.Text = Converter.StringToMoney(row["employee_commission"].ToString()).ToString();
                    tbShippingFee.Text = Converter.StringToMoney(row["shipping_fee"].ToString()).ToString();
                    tbRtsMargin.Text = Converter.StringToPercentage(row["rts_margin"].ToString()).ToString();
                }

                tbCostOfGood.IsReadOnly = true;
                tbCostOfGood.Focusable = false;
                tbFactor.Focus();
            }
            else
            {
                tbCostOfGood.IsReadOnly = false;
                tbCostOfGood.Focusable = true;
                tbCostOfGood.Focus();
                btnUpdate.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 1)
            {
                if (product != null)
                {
                    product.NominatedPrice = Converter.StringToDecimal(string.IsNullOrEmpty(tbNominated.Text.Replace(",", "")) ? "0" : tbNominated.Text.Replace(",", ""));
                    this.DialogResult = true;
                }
            }
            else
            {
                if ((cbItems.SelectedItem != null || cbItems.Text != "Choose Item") && mode == 2 && ((Product)cbItems.SelectedItem).ItemName != "None")
                {

                    DBHelper db = new DBHelper();

                    string productId = ((Product)cbItems.SelectedItem).ProductId;

                    bool updateNomiatedPrice = await db.UpdateData("tbl_products", new string[] { "nominated_price" }, new string[] { Converter.StringToDecimal(tbNominated.Text).ToString() }, "product_id", productId);

                    string? updateSellingExpenses = await db.InsertOrUpdateData("tbl_selling_expenses",
                                                                             new string[] { "ads_budget", "roas", "adspent_per_item", "platform_commission", "employee_commission", "shipping_fee", "rts_margin" },
                                                                             new string[] { 
                                                                                 Converter.StringToDecimal(tbAdsBudget.Text).ToString(),
                                                                                 Converter.StringToInteger(tbROAS.Text).ToString(),
                                                                                 Converter.StringToDecimal(tbAdsSpent.Text).ToString(),
                                                                                 Converter.StringToDecimal(tbPlatform.Text).ToString(),
                                                                                 Converter.StringToDecimal(tbEmployeeCommission.Text).ToString(),
                                                                                 Converter.StringToDecimal(tbShippingFee.Text).ToString(),
                                                                                 Converter.StringToDecimal(tbRtsMargin.Text).ToString()
                                                                             }, "product_id", productId);
                    
                    
                    if (updateNomiatedPrice && updateSellingExpenses != null)
                    {
                        MessageBox.Show("Nominated Price Updated Succesfully");
                    }
                }
            }
        }

        private void btnEditProvision_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tbTotalAdministrative.IsEnabled == true)
            {
                tbTotalAdministrative.IsEnabled = false;
                tbUnitSold.IsEnabled = false;
            }
            else
            {
                tbTotalAdministrative.IsEnabled = true;
                tbUnitSold.IsEnabled = true;
            }
        }

        private void tbFactor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbFactor.IsFocused)
            {
                Util.HideToolTip(tbFactor);
                tbNominated.Text = Util.calculateTextBox(tbCostOfGood, '*', tbFactor);
            }
        }

        private void tbFactor_LostFocus(object sender, RoutedEventArgs e)
        {
            Util.HideToolTip(tbFactor);
            Converter.ToDecimal(tbFactor);
        }

        private void tbNominated_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbGrossPerMonth.Text = Util.calculateTextBox(tbNominated, '*', tbQuantity);
            tbNetProfit.Text = Util.calculateTextBox(tbNominated, '-', tbTotalCost);
            calculateSellingExpenses();
        }

        private void tbNominated_LostFocus(object sender, RoutedEventArgs e)
        {
            tbFactor.Text = Util.calculateTextBox(tbNominated, '/', tbCostOfGood);
            Converter.ToDecimal(tbFactor);
            Converter.ToMoney(tbNominated);
        }

        private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbGrossPerMonth.Text = Util.calculateTextBox(tbNominated, '*', tbQuantity);
            tbNetProfitPerMonth.Text = Util.calculateTextBox(tbNetProfit, '*', tbQuantity);
        }

        private void tbAdsBudget_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbAdsSpent.Text = Util.calculateTextBox(tbAdsBudget, '/', tbROAS);
        }

        private void tbROAS_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbAdsSpent.Text = Util.calculateTextBox(tbAdsBudget, '/', tbROAS);
        }

        private void tbAdsSpent_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateSellingExpenses();
        }

        private void tbPlatform_TextChanged(object sender, TextChangedEventArgs e)
        {
            Util.HideToolTip(tbPlatform);
            calculateSellingExpenses();
        }

        private void tbEmployeeCommission_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateSellingExpenses();
        }

        private void tbShippingFee_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateSellingExpenses();
        }

        private void tbRtsMargin_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateSellingExpenses();
        }

        private void calculateSellingExpenses()
        {
            decimal rtsTotal = 0, platformTotal = 0, nominatedPrice = 0;

            if (decimal.TryParse(tbNominated.Text.Replace(",", ""), out decimal nominated))
            {
                nominatedPrice = nominated;
            }

            if (decimal.TryParse(tbRtsMargin.Text.ToString().Replace("%", ""), out decimal rtsDecimal))
            {
                rtsTotal = nominatedPrice * (rtsDecimal / 100);
            }

            if (decimal.TryParse(tbPlatform.Text.Replace("%", ""), out decimal platform))
            {
                platformTotal = nominatedPrice * (platform / 100);
            }

            decimal total = Util.SumTextBoxes(tbAdsSpent.Text.Replace(",", ""), tbCostOfGood.Text.Replace(",", ""), platformTotal, tbEmployeeCommission.Text.Replace(",", ""), tbShippingFee.Text.Replace(",", ""), rtsTotal);
            tbSellingTotal.Text = total.ToString("N2");

        }

        private void tbSellingTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbTotalCost.Text = Util.calculateTextBox(tbCostPerUnit, '+', tbSellingTotal);
        }

        private void tbTotalAdministrative_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbCostPerUnit != null)
            {
                tbCostPerUnit.Text = Util.calculateTextBox(tbTotalAdministrative, '/', tbUnitSold);
            }
        }

        private void tbUnitSold_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbCostPerUnit != null)
            {
                tbCostPerUnit.Text = Util.calculateTextBox(tbTotalAdministrative, '/', tbUnitSold);
            }
        }

        private void tbCostPerUnit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTotalCost != null)
            {
                tbTotalCost.Text = Util.calculateTextBox(tbSellingTotal, '+', tbCostPerUnit);
            }
        }

        private void tbTotalCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbNetProfit.Text = Util.calculateTextBox(tbNominated, '-', tbTotalCost);
        }

        private void tbNetProfit_TextChanged(object sender, TextChangedEventArgs e)
        {
            Util.ValidateIfProfitable(tbNetProfit);
            if (tbNetProfitPerMonth != null)
            {
                tbNetProfitPerMonth.Text = Util.calculateTextBox(tbNetProfit, '*', tbQuantity);
            }
        }

        private void tbNetProfitPerMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            Util.ValidateIfProfitable(tbNetProfitPerMonth);
        }

        private void tbGrossPerMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            Util.ValidateIfProfitable(tbGrossPerMonth);
        }

        private void moneyLostFocus(object sender, RoutedEventArgs e)
        {
            Converter.ToMoney(sender as TextBox);
        }

        private void percentLostFocus(object sender, RoutedEventArgs e)
        {
            Util.HideToolTip(sender as TextBox);
            Converter.ToPercent(sender as TextBox);
        }

        private void numWithCommaLostFocus(object sender, RoutedEventArgs e)
        {
            Converter.ToNumWithComma(sender as TextBox);
        }

        private void textBoxGotFocus(object sender, RoutedEventArgs e)
        {
            Util.HideToolTip(sender as TextBox);
            Converter.ToNumber(sender as TextBox);
        }

        private void tooltipGotFocus(object sender, RoutedEventArgs e)
        {
            Util.setTextBoxToolTip(sender as TextBox, "Optional");
            Converter.ToNumber(sender as TextBox);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Decimal(sender, e);
        }
        private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }

        private void percentageValidation(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Percentage(sender, e);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
