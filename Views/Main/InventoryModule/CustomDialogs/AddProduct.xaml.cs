using IronBarCode;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
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
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace WarehouseManagement.Views.Main.InventoryModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        Bitmap barcodeImage = null;
        Product? product;
        SellingExpenses? sellingExpenses;
        public event EventHandler<string> TableFilterRequested;
        bool isUpdate = false;
        private void OnTableFilterRequested(string status)
        {
            TableFilterRequested?.Invoke(this, status);
        }

        public AddProduct(Product? product)     
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;
            this.product = product;
            SetValues();
        }

        private async void SetValues()
        {
            if (product == null)
            {
                tbProductId.Text = "ECOMM" + new Random().Next(100000000).ToString("D8");
            }
            else
            {
                
                DBHelper db = new();
                isUpdate = true;
                tbProductId.Text = product.ProductId;

                IEnumerable<string> columnNames = new List<string>()
                {
                    "product_id",
                    "item_name",
                    "acq_cost",
                    "nominated_price",
                    "barcode",
                    "unit_quantity",
                };

                Dictionary<string, object>? row = await db.GetRow("tbl_products", columnNames, "product_id", product.ProductId);

                if (row != null && row.Count > 0)
                {

                    tbItemName.Text = row["item_name"].ToString();
                    tbAcquisitionCost.Text = Converter.StringToMoney(row["acq_cost"].ToString()).ToString();
                    tbBarcode.Text = row["barcode"].ToString();
                    tbUnitQuantity.Text = row["unit_quantity"].ToString();
                    tbNominatedPrice.Text = Converter.StringToMoney(row["nominated_price"].ToString()).ToString();
                }

                tbEmployeeCommission.Text = await db.GetValue("tbl_selling_expenses", "employee_commission", "product_id", product.ProductId);

                lbProduct.Text = "Update Product";
                btnProceed.Content = "Update";
            }
        }

        private async void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(tbItemName, tbAcquisitionCost))
            {
                MessageBox.Show("Do not leave required fields empty");
                return;
            }

            Product newProduct = new Product()
            {
                ProductId = tbProductId.Text,
                ItemName = tbItemName.Text,
                AcqCost = Converter.StringToDecimal(tbAcquisitionCost.Text),
                Barcode = string.IsNullOrEmpty(tbBarcode.Text) ? "N/A" : tbBarcode.Text,
                UnitQuantity = Converter.StringToInteger(tbUnitQuantity.Text),
                NominatedPrice = Converter.StringToDecimal(tbNominatedPrice.Text)
            };

            newProduct.Status = newProduct.UnitQuantity < 0 ? Util.status_out_of_stock : newProduct.UnitQuantity == 0 ? Util.status_out_of_stock : newProduct.UnitQuantity <= 100 ? Util.status_low_stock : Util.status_in_stock;

            using DBHelper db = new DBHelper();

            string[] productColumns = { "item_name", "acq_cost", "barcode", "unit_quantity", "nominated_price", "status", "reorder_point", "timestamp" };
            string[] productValues = { newProduct.ItemName, newProduct.AcqCost.ToString(), newProduct.Barcode, newProduct.UnitQuantity.ToString(), newProduct.NominatedPrice.ToString(), newProduct.Status, "100", DateTime.Now.ToString() };

            bool operationResult;

            if (isUpdate)
            {
                operationResult = await db.UpdateData("tbl_products", productColumns, productValues, "product_id", newProduct.ProductId);
            }
            else
            {
                productColumns = new[] { "product_id" }.Concat(productColumns).ToArray();
                productValues = new[] { newProduct.ProductId }.Concat(productValues).ToArray();

                operationResult = await db.InsertData("tbl_products", productColumns, productValues);
            }

            string[] expensesColumns = { "employee_commission" };
            string[] expensesValues = { Converter.StringToDecimal(tbEmployeeCommission.Text).ToString() };

            string? employeeCommissionAdded = await db.InsertOrUpdateData("tbl_selling_expenses", expensesColumns, expensesValues, "product_id", newProduct.ProductId);

            if (operationResult && (employeeCommissionAdded != null))
            {
                if (!isUpdate)
                {
                    if (sellingExpenses != null)
                    {
                        string? updateSellingExpenses = await db.InsertOrUpdateData("tbl_selling_expenses",
                                                                             new string[] { "ads_budget", "roas", "adspent_per_item", "platform_commission", "employee_commission", "shipping_fee", "rts_margin" },
                                                                             new string[] {
                                                                                sellingExpenses.adsBudget.ToString(),
                                                                                sellingExpenses.roas.ToString(),
                                                                                sellingExpenses.adspentPerItem.ToString(),
                                                                                sellingExpenses.platformCommission.ToString(),
                                                                                sellingExpenses.employeeCommission.ToString(),
                                                                                sellingExpenses.shippingFee.ToString(),
                                                                                sellingExpenses.rtsMargin.ToString(),
                                                                             }, "product_id", newProduct.ProductId);
                    }
                    //For generating barcode
                    //GeneratedBarcode myBarcode = IronBarCode.BarcodeWriter.CreateBarcode(tbBarcode.Text, BarcodeWriterEncoding.Code128);
                    //myBarcode.SaveAsPng($"./images/{tbItemName.Text}.png");

                    //Clear();
                    //OnTableFilterRequested(null);

                }
                else
                {
                    //GeneratedBarcode myBarcode = IronBarCode.BarcodeWriter.CreateBarcode(tbBarcode.Text, BarcodeWriterEncoding.Code128);
                    //myBarcode.SaveAsPng($"./images/{tbItemName.Text}.png");
                    MessageBox.Show("Product updated successfully");
                   
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show(isUpdate ? "Error updating product" : "Error inserting product");
            }
        }

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(tbItemName, tbAcquisitionCost, tbUnitQuantity))
            {
                MessageBox.Show("Please provide the Item Name, Acquisition Cost, and Unit Quantity to perform profit analysis.");
                return;
            }

            Product product = new()
            {
                ItemName = tbItemName.Text,
                AcqCost = Converter.StringToDecimal(tbAcquisitionCost.Text),
                NominatedPrice = Converter.StringToDecimal(tbNominatedPrice.Text),
                UnitQuantity = Converter.StringToInteger(tbUnitQuantity.Text)
            };

            ProfitAnalysis pa = new ProfitAnalysis(product)
            {
                mode = 1
            };

            if (pa.ShowDialog() == true)
            {
                tbNominatedPrice.Text = pa.product.NominatedPrice.ToString();
                sellingExpenses = pa.sellingExpenses;
                if (sellingExpenses != null)
                {
                    tbEmployeeCommission.Text = sellingExpenses.employeeCommission.ToString();
                }
            }
        }


        private void Clear()
        {
            tbProductId.Text = "ECOMM" + new Random().Next(100000000).ToString("D8");
            tbItemName.Clear();
            tbAcquisitionCost.Clear();
            tbBarcode.Clear();
            tbUnitQuantity.Clear();
            tbNominatedPrice.Clear();
            tbEmployeeCommission.Clear();
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Decimal(sender, e);
        }
        private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isUpdate) {
                this.DialogResult = true;
            }
        }

      

        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
           
        }


        private void tbItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            tbItemName.Text = Converter.CapitalizeWords(tbItemName.Text, 2);
        }
    }
}
