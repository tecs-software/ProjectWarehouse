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

            string horizontalBarcodeValue = "9800123763713"; // Replace with your desired value
            string verticalBarcodeValue = "9800123763713"; // Replace with your desired value
            string QRcodeValue = "9800123763713";

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

            ReportParameter parameter1 = new ReportParameter("SortingCode_params", "123-730002");
            ReportParameter parameter2 = new ReportParameter("SortingNo_params", "B123");

            // Create report parameters with your byte arrays
            ReportParameter Hbarcode = new ReportParameter("HBarcode_params", Convert.ToBase64String(horizontalBarcodeBytes));
            ReportParameter Vbarcode = new ReportParameter("VBarcode_params", Convert.ToBase64String(verticalBarcodeBytes));
            ReportParameter WQrcode = new ReportParameter("QRcode_params", Convert.ToBase64String(QRcodeBytes));

            ReportViewer1.LocalReport.SetParameters(new[] { parameter1, parameter2, Hbarcode, Vbarcode, WQrcode });

            ReportViewer1.RefreshReport();

            // Create a PrintDocument for printing
            //PrintDocument printDoc = new PrintDocument();
            //printDoc.PrintPage += (sender, e) =>
            //{
            //    // Render the report as an image and draw it directly on the PrintPageEventArgs
            //    var imageBytes = ReportViewer1.LocalReport.Render("Image");
            //    using (var stream = new System.IO.MemoryStream(imageBytes))
            //    {
            //        using (var image = System.Drawing.Image.FromStream(stream))
            //        {
            //            e.Graphics.DrawImage(image, e.PageBounds);
            //        }
            //    }
            //};

            //// Set the printer name (you can retrieve available printer names using PrinterSettings)
            //printDoc.PrinterSettings.PrinterName = "ZIJIANG LABEL(1)";

            //// Print the document
            //printDoc.Print();
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
