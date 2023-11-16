using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WWarehouseManagement.Database;
using System.Windows.Data;
using System.ComponentModel;

namespace WarehouseManagement.Controller
{
    public class WaybillController
    {
        static sql_control sql = new sql_control();
        public static async Task Insert(
            string Order_id,
            string Waybill,
            string SortingCode,
            string SortingNo,
            string receiverName,
            string receiverProvince,
            string receiverCity,
            string receiverBarangay,
            string receiverAddress,
            string senderName,
            string senderAddress,               
            decimal? cod,
            string goods,
            decimal? price,
            decimal? weight,
            string remarks
            )
        {
            sql.AddParam("@item", goods);
            int sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_products WHERE item_name = @item"));
            string sender_name = sql.ReturnResult($"SELECT sender_name FROM tbl_sender WHERE sender_id = {sender_id}");
            string sender_add = sql.ReturnResult($"SELECT sender_address FROM tbl_sender WHERE sender_id = {sender_id}");


            sql.AddParam("@sender_name", sender_name);
            sql.AddParam("@sender_add", sender_add);
            sql.AddParam("@remarks",remarks);
            sql.AddParam("@rAddress", receiverAddress);
            await Task.Run(() => sql.Query($"INSERT INTO tbl_waybill (Order_ID, Waybill, Sorting_Code, Sorting_No, ReceiverName,ReceiverProvince, ReceiverCity, ReceiverBarangay,ReceiverAddress,SenderName,SenderAddress,COD, Goods, Price, Weight, Remarks) " +
                $"VALUES ('{Order_id}', '{Waybill}', '{SortingCode}', '{SortingNo}', '{receiverName}', '{receiverProvince}', '{receiverCity}', '{receiverBarangay}', @rAddress,  " +
                $" @sender_name, @sender_add, {cod}, '{goods}', {price},{weight}, @remarks) "));
            if (sql.HasException(true)) return;
        }
        public static async Task LoadDevice(ComboBox cmbJnt, ComboBox cmbFlash)
        {
            await Task.Run(() =>
            {
                sql.Query($"SELECT * FROM tbl_printer_setting");
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (dr[2].ToString() == "JNT")
                            {
                                cmbJnt.Text = dr[1].ToString();
                            }
                            else if (dr[2].ToString() == "Flash")
                            {
                                cmbFlash.Text = dr[1].ToString();
                            }
                        });
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        cmbJnt.SelectedIndex = -1;
                        cmbFlash.SelectedIndex = -1;
                    });
                }
            });
        }
        public static async Task Save(String jnt, String flash)
        {
            await Task.Run(() =>
            {
                sql.Query($"SELECT * FROM tbl_printer_setting");
                if (sql.DBDT.Rows.Count > 0)
                {
                    foreach (DataRow dr in sql.DBDT.Rows)
                    {
                        if (dr[2].ToString() == "JNT")
                            sql.Query($"UPDATE tbl_printer_setting SET Name = '{jnt}' WHERE CourierName = 'JNT' ");

                        else if (dr[2].ToString() == "Flash")
                            sql.Query($"UPDATE tbl_printer_setting SET Name = '{flash}' WHERE CourierName = 'Flash' ");

                        if (sql.HasException(true)) return;
                    }
                }
                else
                {
                    sql.Query($"INSERT INTO tbl_printer_setting (Name, CourierName) VALUES ('{jnt}','JNT')");
                    if (sql.HasException(true)) return;

                    sql.Query($"INSERT INTO tbl_printer_setting (Name, CourierName) VALUES ('{flash}','Flash')");
                    if (sql.HasException(true)) return;
                }
            });
        }
        public static async Task<bool> IsEmpty()
        {
            bool condition = false;
            await Task.Run(() =>
            {
                sql.Query($"SELECT * FROM tbl_printer_setting");
                if (sql.DBDT.Rows.Count > 0)
                    condition = false;
                else
                {
                    condition =  true;
                }
            });
            return condition;
        }
        public static async Task DisplayDataOnWaybillJournal(DataGrid dg)
        {
            await Task.Run(() =>
            {
                sql.Query($"SELECT * FROM tbl_waybill ORDER BY ID DESC");

                if (sql.DBDT.Rows.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        List<waybillData> waybill = new List<waybillData>();
                        foreach (DataRow dr in sql.DBDT.Rows)
                        {
                            waybillData details = new waybillData
                            {
                                Order_id = dr[1].ToString(),
                                Waybill = dr[2].ToString(),
                                Receiver = dr[5].ToString(),
                                Date = dr[0].ToString(),
                                Remarks = dr[16].ToString()

                            };
                            waybill.Add(details);
                        }
                        dg.ItemsSource = waybill;
                    });
                }
            });
        }
        public static void searchWaybill(TextBox tb, DataGrid dg)
        {
            sql.Query($"SELECT * FROM tbl_waybill WHERE Waybill LIKE '%{tb.Text}%'");
            if (sql.DBDT.Rows.Count > 0)
            {
                Application.Current.Dispatcher.Invoke(() => dg.ItemsSource = sql.DBDT.DefaultView);
            }
        }
    }
    public class waybillData : INotifyPropertyChanged
    {
        private bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Order_id { get; set; } = string.Empty;
        public string Waybill { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

}
