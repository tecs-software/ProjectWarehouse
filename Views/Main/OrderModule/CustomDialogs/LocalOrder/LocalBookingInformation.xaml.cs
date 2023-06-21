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
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.LocalOrder
{
    /// <summary>
    /// Interaction logic for LocalBookingInformation.xaml
    /// </summary>
    public partial class LocalBookingInformation : Page
    {

        decimal? itemPrice = 0;

        public LocalBookingInformation()
        {
            InitializeComponent();
            SetItems();
        }

        public async void SetItems()
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
                cbItem.ItemsSource = products;
                cbItem.DisplayMemberPath = "ItemName";
            }
        }

        private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }

        private void cbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbItem.SelectedItem is Product selectedProduct)
            {
              
                decimal? nominatedPrice = selectedProduct.NominatedPrice;
                itemPrice = nominatedPrice;
                int? quantity = Converter.StringToInteger(tbQuantity.Text);

                decimal total = nominatedPrice ?? 0 * quantity ?? 0;

                tbTotal.Text = $"₱{total:N2}";
            }
        }

        private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(cbItem.Text))
            {
                return;
            }

            int? quantity = Converter.StringToInteger(tbQuantity.Text);

            decimal? total = quantity * itemPrice;

            tbTotal.Text = $"₱{total:N2}";
        }
    }
}
