using Microsoft.Reporting.WinForms;
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
using System.Drawing.Printing;
using ZXing.Windows.Compatibility;
using ZXing.Rendering;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Data;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Views.Main.SystemSettingModule
{
    /// <summary>
    /// Interaction logic for WaybillView.xaml
    /// </summary>
    public partial class WaybillView : Window
    {
        public WaybillView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReportViewer1.LocalReport.ReportEmbeddedResource = "WarehouseManagement.Waybill.WaybillTemplate.rdlc";
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.RefreshReport();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
            sql.Query($"SELECT TOP 1 * FROM tbl_waybill ORDER BY ID DESC");
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
                    ReportParameter sortingCode = new ReportParameter("SortingCode_params", dr[3].ToString());
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
                    ReportParameter date = new ReportParameter("Date_params", DateTime.Now.ToString("YYYY/MM/DD"));
                    ReportParameter time = new ReportParameter("Time_params", DateTime.Now.ToString("hh:mm:ss"));

                    // images(Barcodes/QR code)
                    ReportParameter Hbarcode = new ReportParameter("HBarcode_params", Convert.ToBase64String(horizontalBarcodeBytes));
                    ReportParameter Vbarcode = new ReportParameter("VBarcode_params", Convert.ToBase64String(verticalBarcodeBytes));
                    ReportParameter WQrcode = new ReportParameter("QRcode_params", Convert.ToBase64String(QRcodeBytes));

                    ReportViewer1.LocalReport.SetParameters(new[] { sortingCode, sortingNo, Hbarcode, Vbarcode, WQrcode, waybill, receiver_barangay, receiver_name,
                    receiver_address, sender_name, sender_address, cod, goods, price, weight, remarks, order_id, date, time});

                    ReportViewer1.RefreshReport();

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
                    printDoc.PrinterSettings.PrinterName = "ZIJIANG LABEL";

                    // Print the document
                    printDoc.Print();
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

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {

            LocalReport localReport = new LocalReport();
            localReport.ReportEmbeddedResource = "WarehouseManagement.Waybill.WaybillTemplate.rdlc";

            // Set parameter values
            
        }
    }
}
