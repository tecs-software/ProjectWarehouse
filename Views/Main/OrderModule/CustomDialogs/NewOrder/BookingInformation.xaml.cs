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
            sql.Query($"SELECT item_name FROM tbl_products");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
            {
                foreach(DataRow dr in sql.DBDT.Rows)
                {
                    cbItem.Items.Add(dr[0]);
                }
            }
        }
    }
}
