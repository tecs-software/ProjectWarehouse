using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WarehouseManagement.Controller;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder;
using WWarehouseManagement.Database;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for BulkOrderPopup.xaml
    /// </summary>
    public partial class BulkOrderPopup : Window
    {
        Dictionary<string, bulk_model> bulkDictionary;
        BackgroundWorker pushOrders;
        void CustomMessageBox(String message, Boolean questionType)
        {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            txtMessageDialog.Text = message;
            if (questionType)
            {
                btnYes.Content = "Yes";
                btnNo.Visibility = Visibility.Visible;
            }
            else
            {
                btnYes.Content = "Okay";
                btnNo.Visibility = Visibility.Collapsed;
            }
            dialog.IsOpen = true;
        }
        public BulkOrderPopup()
        {
            InitializeComponent();
            dtSuspiciousOrders.Visibility = Visibility.Collapsed;
            Csv_Controller.dataTableBulkOrders = null;
            Csv_Controller.model = new List<bulk_model>();
            Csv_Controller.Fmodels = new List<FLASHModel>();
            rdbJandT.IsChecked = true;
            bulkDictionary = new Dictionary<string, bulk_model>();
        }
        Create_api bulk_api = new Create_api();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            bulk_inserts.delete_temp_table();
            this.DialogResult = true;
            Close();
        }
        

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    //txtFileNameProduct.Text = openFileDialog.FileName;
                    Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                    int numberofitems = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName).Rows.Count;
                    pbBulkOrder.Maximum = numberofitems > 0 ? numberofitems : 100;
                    Csv_Controller.dataTablebulkOrder = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                    dtBulkOrders.ItemsSource = Csv_Controller.dataTablebulkOrder.DefaultView;

                    dtBulkOrders.Visibility = Visibility.Visible;
                    dtSuspiciousOrders.Visibility = Visibility.Collapsed;

                    //dtBulkOrders.Columns[2].IsReadOnly = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem() { Header = "Delete Row" };

            Util.ShowContextMenuForButton(sender as Button, item1);

            item1.Click += Delete_Row_Click;
        }
        private async void Delete_Row_Click(object sender, RoutedEventArgs e)
        {
            object id = dtSuspiciousOrders.SelectedItem;
            string selectedID = (dtSuspiciousOrders.SelectedCells[1].Column.GetCellContent(id) as TextBlock).Text;
            bulk_inserts.delete_suspicious_row(int.Parse(selectedID));
            bulk_inserts.show_new_temp_table(dtBulkOrders, dtSuspiciousOrders);

        }

        private void dtBulkOrders_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(decimal))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = $"#,##0.#0";

                }
            }
        }
        public static ComboBox cb { get; set; }
        public static TextBox tb { get; set; }
        private void cmbSellerName_Loaded(object sender, RoutedEventArgs e)
        {
            string idValue = "";
            cb = sender as ComboBox;
            Csv_Controller.insertItems(cb);
            DataGridCell dataGridCell = FindParent<DataGridCell>(cb);

            if (dataGridCell != null)
            {
                DataGridRow dataGridRow = FindParent<DataGridRow>(dataGridCell);
                if (dataGridRow != null)
                {
                    int columnIndex = 2; // Adjust this to the actual index of the ComboBox column
                    DataGridCellInfo cellInfo = new DataGridCellInfo(dataGridRow.Item, dtBulkOrders.Columns[columnIndex]);
                    DataGridCell cell = GetCell(dtBulkOrders, cellInfo);
                    if (cell != null && cell.Content is TextBlock textBlock)
                    {
                        //idValue = textBlock.Text;
                        //if (bulkDictionary.ContainsKey(idValue))
                        //    //cb.Text = bulkDictionary[idValue].item_name;
                        //else
                        //    cb.SelectedIndex = -1;
                    }
                }

            }

        }
        private void txtQuantity_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            DataGridCell dataGridCell = FindParent<DataGridCell>(tb);

            if (dataGridCell != null)
            {
                DataGridRow dataGridRow = FindParent<DataGridRow>(dataGridCell);

                if (dataGridRow != null)
                {
                    int columnIndex = 2;
                    DataGridCellInfo cellInfo = new DataGridCellInfo(dataGridRow.Item, dtBulkOrders.Columns[columnIndex]);
                    DataGridCell cell = GetCell(dtBulkOrders, cellInfo);
                    if (cell != null && cell.Content is TextBlock textBlock)
                    {
                        //string idValue = textBlock.Text;
                        //if (bulkDictionary.ContainsKey(idValue))
                        //    tb.Text = bulkDictionary[idValue].item_quantity;
                        //else
                        //    tb.Text = string.Empty;
                    }
                }
            }
        }
        public static bool NoError = true;
        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //ReportViewer1.LocalReport.ReportEmbeddedResource = "WarehouseManagement.Waybill.WaybillTemplate.rdlc";
            //ReportViewer1.LocalReport.EnableExternalImages = true;
            //ReportViewer1.RefreshReport();

            if (Csv_Controller.checkItemname(dtBulkOrders) || Csv_Controller.checkNullCells(dtBulkOrders) || Csv_Controller.checkquantity(dtBulkOrders))
            {
                
            }
            else
            {
                if(rdbJandT.IsChecked == true)
                {
                    Csv_Controller.dataTableBulkOrders = Csv_Controller.dataTablebulkOrder;
                    //Push to Create_API
                    foreach (DataRow dr in Csv_Controller.dataTableBulkOrders.Rows)
                    {
                        bulk_model model = new bulk_model()
                        {
                            //receiver payload
                            receiver_name = dr[3].ToString(),
                            receiver_address = dr[5].ToString(),
                            receiver_phone = dr[4].ToString(),
                            receiver_province = dr[6].ToString(),
                            receiver_city = dr[7].ToString(),
                            receiver_area = dr[8].ToString(),

                            //other fields
                            remarks = dr[2].ToString(),
                            product_name = dr[0].ToString(),
                            total = decimal.Parse(dr[13].ToString()),
                            quantity = int.Parse(dr[1].ToString()),

                            //etc
                            cod = decimal.Parse(dr[14].ToString()),
                            parcel_value = decimal.Parse(dr[13].ToString()),
                            parcel_name = dr[10].ToString(),
                            total_parcel = int.Parse(dr[12].ToString()),
                            weight = decimal.Parse(dr[11].ToString())

                        };
                        Csv_Controller.model.Add(model);
                    }
                }
                else
                {
                    Csv_Controller.dataTableBulkOrders = Csv_Controller.dataTablebulkOrder;
                    foreach(DataRow dr in Csv_Controller.dataTableBulkOrders.Rows)
                    {
                        FLASHModel flashmodel = new FLASHModel()
                        {
                            item = dr[0].ToString(),
                            remarks = dr[1].ToString(),
                            receiver_name = dr[2].ToString(),
                            receiver_phone = dr[3].ToString(),
                            receiver_province = dr[4].ToString(),
                            receiver_city = dr[5].ToString(),
                            receiver_barangay = dr[6].ToString(),
                            postal_code = dr[7].ToString(),
                            receiver_address = dr[8].ToString(),
                            article_category = dr[9].ToString(),
                            isCOD = dr[10].ToString(),
                            COD = dr[11].ToString(),
                            weight = dr[12].ToString(),
                            height = dr[13].ToString(),
                            lenght = dr[14].ToString(),
                            width = dr[15].ToString()
                        };
                        Csv_Controller.Fmodels.Add(flashmodel);
                    }
                }
                btnConfirm.IsEnabled = false;
                pushOrders = new BackgroundWorker();
                pushOrders.WorkerReportsProgress = true;

                pushOrders.DoWork += WorkerPushOrders_DoWork;
                pushOrders.RunWorkerCompleted += WorkerPushCompleted_RunWorkerCompleted;

                pushOrders.RunWorkerAsync();
            }
        }
        private async void WorkerPushOrders_DoWork(object sender, DoWorkEventArgs e)
        {
            sql_control sql = new sql_control();

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (rdbJandT.IsChecked == true)
                {
                    GlobalModel.customer_id = sql.ReturnResult($"SELECT customer_id FROM tbl_couriers WHERE courier_name = 'J&T'");
                    GlobalModel.key = sql.ReturnResult($"SELECT api_key FROM tbl_couriers WHERE courier_name = 'J&T'");
                    bulk_api.create_bulk_api(Csv_Controller.model, btnConfirm, false, pbBulkOrder);
                }
                else
                {
                    GlobalModel.customer_id = sql.ReturnResult($"SELECT customer_id FROM tbl_couriers WHERE courier_name = 'FLASH'");
                    GlobalModel.key = sql.ReturnResult($"SELECT api_key FROM tbl_couriers WHERE courier_name = 'FLASH'");
                    FLASH_api.FlashCreateBulkOrder(Csv_Controller.Fmodels, pbBulkOrder);
                }
            });
        }
        private async void WorkerPushCompleted_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await Task.Delay(3000);
            int delayMilliseconds = 1000;
            int maximumAttempts = 100; // Adjust this based on your requirements
            int attempts = 0;

            while (pbBulkOrder.Value < pbBulkOrder.Maximum && attempts < maximumAttempts)
            {
                await Task.Delay(delayMilliseconds);
                attempts++;
            }

            // Now execute your block of code once the progress bar reaches its maximum
            if (pbBulkOrder.Value >= pbBulkOrder.Maximum)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    btnConfirm.IsEnabled = true;
                    MessageBox.Show("Orders have been created.", "Success");
                    bulk_inserts.show_temp_table(dtBulkOrders, dtSuspiciousOrders, btnConfirm, btnReConfirm);
                });
            }
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            //Push to Create_API
            bulk_inserts.load_bulk_model();
            bulk_api.create_bulk_api(Csv_Controller.model, btnReConfirm, true, pbBulkOrder);
            if (NoError)
            {
                btnReConfirm.IsEnabled = true;
                bulk_inserts.delete_temp_table();
                bulk_inserts.show_new_temp_table(dtBulkOrders,dtSuspiciousOrders);
                MessageBox.Show("Orders has been Created");
            }
            else
            {
                btnReConfirm.IsEnabled = true;
            }
        }
        private void cmbSellerName_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            object id = dtBulkOrders.SelectedItem;
            string selectedID = (dtBulkOrders.SelectedCells[2].Column.GetCellContent(id) as TextBlock).Text;

            DataGridRow row = FindVisualParent<DataGridRow>(cmb);

            if (row != null && row.IsSelected)
            {
                DataGridCell cell = GetCell(dtBulkOrders, row, 0);
                TextBox txtQuantity = FindVisualChild<TextBox>(cell);
                string quantity = txtQuantity.Text.ToString();

                bulk_model model = new bulk_model()
                {
                    //ID = selectedID,
                    //item_name = cmb.Text,
                    //item_quantity = quantity
                };
                if (bulkDictionary.ContainsKey(selectedID))
                {
                    bulkDictionary[selectedID] = model;
                }
                else
                {
                    bulkDictionary.Add(selectedID, model);
                }
            }

        }

    

        private void dtSuspiciousOrders_AutoGeneratedColumns(object sender, EventArgs e)
        {
        }

        private void btnReConfirm_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox("Are you sure do you want to push these suspicious orders?",true);
        }

        private void txtQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            tb = sender as TextBox;

            object id = dtBulkOrders.SelectedItem;
            string selectedID = (dtBulkOrders.SelectedCells[2].Column.GetCellContent(id) as TextBlock).Text;

            DataGridRow row = FindVisualParent<DataGridRow>(tb);

            if (row != null && row.IsSelected)
            {
                DataGridCell cell = GetCell(dtBulkOrders, row, 0);
                ComboBox cmbItems = FindVisualChild<ComboBox>(cell);
                string selectedItem = cmbItems.SelectedItem?.ToString();


                bulk_model model = new bulk_model()
                {
                    //ID = selectedID,
                    //item_quantity = tb.Text,
                    //item_name = selectedItem
                };
                if (bulkDictionary.ContainsKey(selectedID))
                {
                    bulkDictionary[selectedID] = model;
                }
                else
                {
                    bulkDictionary.Add(selectedID, model);
                }
            }
            
        }
        private DataGridCell GetCell(DataGrid dataGrid, DataGridRow row, int columnIndex)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }
        public DataGridCell GetCell(DataGrid dataGrid, DataGridCellInfo cellInfo)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    if (presenter != null)
                    {
                        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                        return cell;
                    }
                }
            }
            return null;
        }
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T result)
                {
                    return result;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }
        private T FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T result)
                {
                    return result;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        public T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public async Task printwaybill(string waybill_no)
        {
            sql_control sql = new sql_control();
            BarcodeWriter<Bitmap> horizontalWriter = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Renderer = new BitmapRenderer(),
                Options = new QrCodeEncodingOptions
                {
                    PureBarcode = true, // Set this to true to generate a barcode without text
                    Width = 300, // Adjust the width as needed
                    Height = 150, // Adjust the height as needed
                }
            };

            BarcodeWriter<Bitmap> verticalWriter = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Renderer = new BitmapRenderer(),
                Options = new QrCodeEncodingOptions
                {
                    PureBarcode = true, // Set this to true to generate a barcode without text
                    Width = 150, // Adjust the width as needed
                    Height = 300, // Adjust the height as needed
                }
            };
            BarcodeWriter<Bitmap> QRcode = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.QR_CODE,
                Renderer = new BitmapRenderer(),
                Options = new QrCodeEncodingOptions
                {
                    PureBarcode = true, // Set this to true to generate a barcode without text
                    Width = 150, // Adjust the width as needed
                    Height = 300, // Adjust the height as needed
                }
            };
            sql.Query($"SELECT TOP 1 * FROM tbl_waybill WHERE Waybill = '{waybill_no}'");
            if (sql.HasException(true)) return;
            if (sql.DBDT.Rows.Count > 0)
            {
                foreach (DataRow dr in sql.DBDT.Rows)
                {
                    string horizontalBarcodeValue = dr[2].ToString(); // Replace with your desired value
                    string verticalBarcodeValue = dr[2].ToString(); // Replace with your desired value
                    string QRcodeValue = dr[2].ToString();

                    var horizontalBitmap = horizontalWriter.Write(horizontalBarcodeValue);
                    var verticalBitmap = verticalWriter.Write(verticalBarcodeValue);
                    var QRcodeBitmap = QRcode.Write(QRcodeValue);

                    verticalBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

                    byte[] horizontalBarcodeBytes = ImageToByteArray(horizontalBitmap);
                    byte[] verticalBarcodeBytes = ImageToByteArray(verticalBitmap);
                    byte[] QRcodeBytes = ImageToByteArray(QRcodeBitmap);

                    string reportFilePath = "WarehouseManagement.Waybill.WaybillTemplate.rdlc";
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.LocalReport.ReportEmbeddedResource = reportFilePath;

                    // Waybill Details
                    ReportParameter sortingCode = new ReportParameter("SortingCode_params", dr[3].ToString() + "-" + dr[4].ToString());
                    ReportParameter sortingNo = new ReportParameter("SortingNo_params", dr[4].ToString());
                    ReportParameter waybill = new ReportParameter("Waybill_params", dr[2].ToString());
                    ReportParameter receiver_barangay = new ReportParameter("Receiver_barangay_params", dr[8].ToString());
                    ReportParameter receiver_name = new ReportParameter("Receiver_name_params", dr[5].ToString());
                    ReportParameter receiver_address = new ReportParameter("Receiver_address_params", dr[6].ToString() + "," + dr[7].ToString() + "," + dr[8].ToString() + "," + dr[9].ToString());
                    ReportParameter sender_name = new ReportParameter("Sender_name_params", dr[10].ToString());
                    ReportParameter sender_address = new ReportParameter("Sender_address_params", dr[11].ToString());
                    ReportParameter cod = new ReportParameter("COD_params", dr[12].ToString());
                    ReportParameter goods = new ReportParameter("Goods_params", dr[13].ToString());
                    ReportParameter price = new ReportParameter("Price_params", dr[14].ToString());
                    ReportParameter weight = new ReportParameter("Weight_params", dr[15].ToString());
                    ReportParameter remarks = new ReportParameter("Remarks_params", dr[16].ToString());
                    ReportParameter order_id = new ReportParameter("Order_id_params", dr[1].ToString());
                    ReportParameter date = new ReportParameter("Date_params", DateTime.Now.ToString("yyyy/MM/dd"));
                    ReportParameter time = new ReportParameter("Time_params", DateTime.Now.ToString("hh:mm:ss"));

                    // images(Barcodes/QR code)
                    ReportParameter Hbarcode = new ReportParameter("HBarcode_params", Convert.ToBase64String(horizontalBarcodeBytes));
                    ReportParameter Vbarcode = new ReportParameter("VBarcode_params", Convert.ToBase64String(verticalBarcodeBytes));
                    ReportParameter WQrcode = new ReportParameter("QRcode_params", Convert.ToBase64String(QRcodeBytes));

                    ReportViewer1.LocalReport.SetParameters(new[] { sortingCode, sortingNo, Hbarcode, Vbarcode, WQrcode, waybill, receiver_barangay, receiver_name,
                    receiver_address, sender_name, sender_address, cod, goods, price, weight, remarks, order_id, date, time});

                    ReportViewer1.RefreshReport();

                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // Create a PrintDocument for printing
                            PrintDocument printDoc = new PrintDocument();
                            printDoc.PrintPage += (sender, e) =>
                            {
                                // Render the report as an image and draw it directly on the PrintPageEventArgs
                                var imageBytes = ReportViewer1.LocalReport.Render("Image");
                                using (var stream = new System.IO.MemoryStream(imageBytes))
                                {
                                    using (var image = System.Drawing.Image.FromStream(stream))
                                    {
                                        e.Graphics.DrawImage(image, e.PageBounds);
                                    }
                                }
                            };

                            // Set the printer name (you can retrieve available printer names using PrinterSettings)
                            string? printer = sql.ReturnResult($"SELECT Name FROM tbl_printer_setting WHERE CourierName = 'JNT'");
                            printDoc.PrinterSettings.PrinterName = printer;

                            // Print the document
                            printDoc.Print();
                        });
                    }
                    catch (System.Drawing.Printing.InvalidPrinterException)
                    {
                        // Handle the case where the specified printer cannot be located
                        // Save the report as a PDF on the desktop as a fallback
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        var imageBytes = ReportViewer1.LocalReport.Render("Image");
                        string pngFilePath = System.IO.Path.Combine(desktopPath, dr[2].ToString() + ".png");
                        using (var imageStream = new MemoryStream(imageBytes))
                        using (var image = System.Drawing.Image.FromStream(imageStream))
                        {
                            image.Save(pngFilePath, ImageFormat.Png);
                        }
                        MessageBox.Show("Printer not detected, printed waybill will be saved on your desktop.");
                    }
                }
            }
        }
        private byte[] ImageToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png); // You can change the format as needed (e.g., ImageFormat.Jpeg)
                return stream.ToArray();
            }
        }

        private void rdbJandT_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rdbFlash_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
