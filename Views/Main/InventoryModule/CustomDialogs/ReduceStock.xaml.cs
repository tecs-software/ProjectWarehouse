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
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.InventoryModule.CustomDialogs
{

  

    public partial class ReduceStock : Window
    {

        public Product? product;

        public ReduceStock(Product? product)
        {
            InitializeComponent();
            this.product = product;
            SetValues();
        }

        private void SetValues()
        {
            if (product != null)
            {
                lblItemName.Text = $"Item: {product.ItemName}";
                lblStock.Text = $"Stock: {product.UnitQuantity.ToString()}";
            }
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Decimal(sender, e);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(tbAmount.Text.Replace(",", ""), out int result))
            {
                if (result > product.UnitQuantity)
                {
                    MessageBox.Show("Invalid Amount");
                    return;
                }
            }

            product.UnitQuantity = product.UnitQuantity - result;

            MessageBoxResult mes = MessageBox.Show($"Are you sure you want to deduct {result} stock/s from this item?", "Confirm Deduction", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mes == MessageBoxResult.No)
            {
                e.Handled = true;
                return;
            }

            DialogResult = true;
            Close();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
