using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarehouseManagement.Controller;
using WWarehouseManagement.Database;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for WaybillJournal.xaml
    /// </summary>
    public partial class WaybillJournal : UserControl
    {
        public WaybillJournal()
        {
            InitializeComponent();
            displayWaybillData();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReportViewer1.LocalReport.ReportEmbeddedResource = "WarehouseManagement.Waybill.WaybillTemplate.rdlc";
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.RefreshReport();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            printwaybill();
        }
        private byte[] ImageToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png); // You can change the format as needed (e.g., ImageFormat.Jpeg)
                return stream.ToArray();
            }
        }
        private async void displayWaybillData()
        {
            await WaybillController.DisplayDataOnWaybillJournal(tblWaybilldata);
        }
        private void printwaybill()
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
            sql.Query($"SELECT TOP 1 * FROM tbl_waybill WHERE Waybill = '{waybill}'");
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
        private string waybill { get; set; }
        private void tblWaybilldata_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tblWaybilldata.SelectedItem != null)
            {
                // Assuming your ID column is named "ID" in your DataTable
                DataRowView selectedRow = tblWaybilldata.SelectedItem as DataRowView;
                if (selectedRow != null)
                {
                    // Access the ID and set it to the TextBox
                    waybill = selectedRow["Waybill"].ToString();
                    btnPrint.Content = "Print-" + waybill;
                }
            }
            else
            {
                // No item is selected, clear the TextBox
                txtSearch.Clear();
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            WaybillController.searchWaybill(txtSearch, tblWaybilldata);
        }
    }
}
