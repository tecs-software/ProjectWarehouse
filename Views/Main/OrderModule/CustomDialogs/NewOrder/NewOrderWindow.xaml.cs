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
using WarehouseManagement.Controller;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using System.Drawing.Printing;
using ZXing.Windows.Compatibility;
using ZXing.Rendering;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Reporting.WinForms;
using WWarehouseManagement.Database;
using System.Data;

namespace WarehouseManagement.Views.Main.OrderModule.CustomDialogs.NewOrder
{
    /// <summary>
    /// Interaction logic for NewOrderWindow.xaml
    /// </summary>
    /// 
    
    public partial class NewOrderWindow : Window
    {
        private SenderInformation senderInformationPage;
        private ReceiverInformation receiverInformationPage;
        private BookingInformation bookingInformationPage;

        //models
        Receiver _receiver = new Receiver();
        Booking_info booking_info = new Booking_info();
        Create_api order_api = new Create_api();
        db_queries queries = new db_queries();
        FLASHModel _flashmodel = new FLASHModel();

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
        private byte[] ImageToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png); // You can change the format as needed (e.g., ImageFormat.Jpeg)
                return stream.ToArray();
            }
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
            sql.Query($"SELECT TOP 1 * FROM tbl_waybill ORDER BY ID DESC");
            if (sql.HasException(true)) return;
            if(sql.DBDT.Rows.Count > 0)
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

        public NewOrderWindow()
        {
            InitializeComponent();
            mainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(GetOrCreateReceiverInformationPage());
            this.SizeToContent = SizeToContent.Height;
        }
        SuspiciousController controller = new SuspiciousController();
        public bool emptyFieldsChecker()
        {
            if (Util.IsAnyTextBoxEmpty(receiverInformationPage.tbFullName, receiverInformationPage.tbPhone, receiverInformationPage.tbAddress) || Util.IsAnyStringEmpty(receiverInformationPage.tbQuantity.Text, receiverInformationPage.tbGoodsValue.Text, receiverInformationPage.cbItem.Text, receiverInformationPage.tbWeight.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (receiverInformationPage.rdbJandT.IsChecked == true)
            {
                db_queries.getCourierDetails("J&T");
                if (emptyFieldsChecker())
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                else
                {
                    _receiver.FirstName = receiverInformationPage.tbFullName.Text;
                    _receiver.Phone = receiverInformationPage.tbPhone.Text;
                    _receiver.Province = receiverInformationPage.cbProvinceJnt.Text;
                    _receiver.City = receiverInformationPage.cbCityJnt.Text;
                    _receiver.Barangay = receiverInformationPage.cbBarangayJnt.Text;
                    _receiver.Address = receiverInformationPage.tbAddress.Text;

                    //booking frame
                    booking_info.item_name = receiverInformationPage.cbItem.Text;
                    booking_info.weight = receiverInformationPage.tbWeight.Text;
                    booking_info.goods_value = receiverInformationPage.tbTotalGoods.Text;
                    booking_info.bag_specification = receiverInformationPage.tbBagSpecification.Text;
                    booking_info.remarks = receiverInformationPage.tbRemarks.Text;
                    booking_info.quantity = receiverInformationPage.tbQuantity.Text;
                    booking_info.cod = decimal.Parse(receiverInformationPage.tbCod.Text);
                    btnNext.IsEnabled = false;
                    //calling the method for api ordering
                    //initialaize rdlc
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "WarehouseManagement.Waybill.WaybillTemplate.rdlc";
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.RefreshReport();
                    if (queries.check_quantity(booking_info, _receiver))
                    {
                        if (await order_api.api_create(_receiver, booking_info, isSuspicious, receiverInformationPage.tbCod.Text))
                        {
                            btnNext.IsEnabled = true;
                            this.DialogResult = true;
                            printwaybill();
                            this.Close();
                        }
                        else
                        {
                            btnNext.IsEnabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Not enought stocks for the desired quantity.");
                    }
                }
            }
            else
            {
                db_queries.getCourierDetails("FLASH");
                if (emptyFieldsChecker())
                {
                    MessageBox.Show("Fill up required fields");
                    return;
                }
                else
                {
                    //receiver details
                    _flashmodel.receiver_name = receiverInformationPage.tbFullName.Text;
                    _flashmodel.receiver_phone = receiverInformationPage.tbPhone.Text;
                    _flashmodel.receiver_province = receiverInformationPage.cbProvinceFlash.Text;
                    _flashmodel.receiver_city = receiverInformationPage.cbCityFlash.Text;
                    _flashmodel.receiver_barangay = receiverInformationPage.cbBarangayFlash.Text;
                    _flashmodel.postal_code = receiverInformationPage.cbPostalCodeFlash.Text;
                    _flashmodel.receiver_address = receiverInformationPage.tbAddress.Text;
                    _flashmodel.weight = receiverInformationPage.tbWeight.Text;

                    //parcel details
                    _flashmodel.remarks = receiverInformationPage.tbRemarks.Text;
                    _flashmodel.COD = receiverInformationPage.tbGoodsValue.Text;
                    _flashmodel.item = receiverInformationPage.cbItem.Text;

                    //sender details
                    sql_control sql = new sql_control();
                    sql.AddParam("@item", _flashmodel.item);
                    int sender_id = int.Parse(sql.ReturnResult($"SELECT sender_id FROM tbl_products WHERE item_name = @item"));
                    sql.Query($"SELECT * FROM tbl_sender WHERE sender_id = {sender_id}");
                    if(sql.DBDT.Rows.Count > 0)
                    {
                        foreach(DataRow dr in sql.DBDT.Rows)
                        {
                            GlobalModel.sender_name = dr[1].ToString();
                            GlobalModel.sender_phone = dr[5].ToString();
                            GlobalModel.sender_province = dr[2].ToString();
                            GlobalModel.sender_city = dr[3].ToString();
                            GlobalModel.sender_address = dr[6].ToString();
                            GlobalModel.sender_postal = dr[7].ToString();
                        }
                    }
                    switch (receiverInformationPage.cbSizeFlash.Text)
                    {
                        case "MINI":
                            _flashmodel.lenght = "12";
                            _flashmodel.width = "20";
                            _flashmodel.height = "6";
                            break;
                        case "SMALL":
                            _flashmodel.lenght = "20";
                            _flashmodel.width = "30";
                            _flashmodel.height = "10";
                            break;
                        case "+SMALL":
                            _flashmodel.lenght = "24";
                            _flashmodel.width = "37";
                            _flashmodel.height = "14";
                            break;
                        case "MEDIUM":
                            _flashmodel.lenght = "27";
                            _flashmodel.width = "43";
                            _flashmodel.height = "20";
                            break;
                        case "+MEDIUM":
                            _flashmodel.lenght = "35";
                            _flashmodel.width = "45";
                            _flashmodel.height = "25";
                            break;
                        case "LARGE":
                            _flashmodel.lenght = "40";
                            _flashmodel.width = "50";
                            _flashmodel.height = "30";
                            break;
                    }
                    switch (receiverInformationPage.cbOrderType.Text)
                    {
                        case "Cash on Delivery (COD)":
                            _flashmodel.isCOD = "1";
                            break;
                        case "Non-Cash on Delivery (Non-COD)":
                            _flashmodel.isCOD = "0";
                            break;
                    }
                    switch (receiverInformationPage.cbItemType.Text)
                    {
                        case "File":
                            _flashmodel.article_category = "0";
                            break;
                        case "Dry food":
                            _flashmodel.article_category = "1";
                            break;
                        case "Commodity":
                            _flashmodel.article_category = "2";
                            break;
                        case "Digital products":
                            _flashmodel.article_category = "3";
                            break;
                        case "Clothes":
                            _flashmodel.article_category = "4";
                            break;
                        case "Books":
                            _flashmodel.article_category = "5";
                            break;
                        case "Auto parts":
                            _flashmodel.article_category = "6";
                            break;
                        case "Shoes and bags":
                            _flashmodel.article_category = "7";
                            break;
                        case "sports equipment":
                            _flashmodel.article_category = "8";
                            break;
                        case "Cosmetics":
                            _flashmodel.article_category = "9";
                            break;
                        case "Household":
                            _flashmodel.article_category = "10";
                            break;
                        case "Fruit":
                            _flashmodel.article_category = "11";
                            break;
                        case "Others":
                            _flashmodel.article_category = "99";
                            break;
                    }
                    if (await FLASH_api.FlashCreateOrder(_flashmodel))
                    {
                        MessageBox.Show("Order success!");
                        btnNext.IsEnabled = true;
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        btnNext.IsEnabled = true;
                    }
                }
            }
        }

        private void btnBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mainFrame.CanGoBack)
            {
                mainFrame.GoBack();
            }
        }

        private SenderInformation GetOrCreateSenderInformationPage()
        {
            if (senderInformationPage == null)
            {
                senderInformationPage = new SenderInformation();
            }

            return senderInformationPage;
        }

        private ReceiverInformation GetOrCreateReceiverInformationPage()
        {
            if (receiverInformationPage == null)
            {
                receiverInformationPage = new ReceiverInformation();
            }

            return receiverInformationPage;
        }

        private BookingInformation GetOrCreateBookingInformationPage()
        {
            if (bookingInformationPage == null)
            {
                bookingInformationPage = new BookingInformation();
            }
            
            return bookingInformationPage;
        }
        public bool isSuspicious { get; set; } = false;
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if(txtMessageDialog.Text == "The data you will send has a matching record in TECS, and has value of RTS. Proceed with the booking?")
            {
                isSuspicious = true;
            }
            mainFrame.Navigate(GetOrCreateBookingInformationPage());
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
