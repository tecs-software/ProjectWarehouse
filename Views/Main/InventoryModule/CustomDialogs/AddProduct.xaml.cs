using Newtonsoft.Json.Bson;
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
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        Product? product;
        bool isUpdate = false;

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
                tbItemName.Text = product.ItemName;
                tbAcquisitionCost.Text = product.AcqCost.ToString();
                tbBarcode.Text = product.Barcode;
                tbUnitQuantity.Text = product.UnitQuantity.ToString();
                tbNominatedPrice.Text = product.NominatedPrice.ToString();
                tbEmployeeCommission.Text = await db.GetValue("tb_selling_expenses", "employee_commission", "product_id", product.ProductId);
            }
        }

        private async void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(tbItemName, tbAcquisitionCost))
            {
                MessageBox.Show("Do not leave required fields empty");
                return;
            }

            Product newProduct = new Product();
            newProduct.ProductId = tbProductId.Text;
            newProduct.ItemName = tbItemName.Text;
            newProduct.AcqCost = Converter.StringToDecimal(tbAcquisitionCost.Text);
            newProduct.Barcode = string.IsNullOrEmpty(tbBarcode.Text) ? "N/A" : tbBarcode.Text;
            newProduct.UnitQuantity = Converter.StringToInteger(tbUnitQuantity.Text);
            newProduct.NominatedPrice = Converter.StringToDecimal(tbNominatedPrice.Text);
            newProduct.Status = newProduct.UnitQuantity < 0 ? Util.status_out_of_stock : newProduct.UnitQuantity == 0 ? Util.status_out_of_stock : newProduct.UnitQuantity <= 100 ? Util.status_low_stock : Util.status_in_stock;

            using DBHelper db = new();

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
                MessageBox.Show(isUpdate ? "Product updated successfully" : "Product inserted successfully");

                if (!isUpdate)
                {
                    Clear();
                }
            }
            else
            {
                MessageBox.Show(isUpdate ? "Error updating product" : "Error inserting product");
            }
        }

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(tbItemName, tbAcquisitionCost))
            {
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
            this.DialogResult = true;
        }

        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbUnitQuantity.Focus();
                e.Handled = true;
            }
        }

        private void tbItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            tbItemName.Text = Converter.CapitalizeWords(tbItemName.Text, 2);
        }
    }
}
