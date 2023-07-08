using System;
using System.Collections.Generic;
using System.Data;
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
using WWarehouseManagement.Database;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder
{
    /// <summary>
    /// Interaction logic for BookingInformation.xaml
    /// </summary>
    public partial class BookingInformation : Page
    {
        sql_control sql = new sql_control();
        public BookingInformation()
        {
            InitializeComponent();
            insert_item();
        }
        public void insert_item()
        {
            if(CurrentUser.Instance.userID == 1)
            {
                sql.Query($"SELECT * FROM tbl_products");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        cbItem.Items.Add(dr[2]);
                    }
                }
            }
            else
            {
                int? sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_users WHERE user_id = {int.Parse(CurrentUser.Instance.userID.ToString())}"));
                sql.Query($"SELECT * FROM tbl_products WHERE sender_id = {sender_id}");
                if (sql.HasException(true)) return;
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        cbItem.Items.Add(dr[2]);
                    }
                }
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadioButton = (RadioButton)sender;
            string selectedRadioButtonName = selectedRadioButton.Content.ToString();
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

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }
    }
}
