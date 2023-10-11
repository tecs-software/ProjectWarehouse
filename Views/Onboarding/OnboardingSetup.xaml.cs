using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using WarehouseManagement.Controller;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Views.Main;
using WWarehouseManagement.Database;
using static WarehouseManagement.Models.Address;

namespace WarehouseManagement.Views.Onboarding
{
    /// <summary>
    /// Interaction logic for OnboardingSetup.xaml
    /// </summary>
    public partial class OnboardingSetup : Window
    {
        BackgroundWorker workerImportAddress;

        sql_control sql = new sql_control();

        public DataTable JNTAddress { get; set; }
        public DataTable FlashAddress { get; set; }
        void CheckedRadio(string text)
        {
            if (text == "J&T")
            {
                ContainerFlash.Visibility = Visibility.Collapsed;
                ContainerJnt.Visibility = Visibility.Visible;
            }
            if (text == "FLASH")
            {
                ContainerFlash.Visibility = Visibility.Visible;
                ContainerJnt.Visibility = Visibility.Collapsed;
            }
        }

        public OnboardingSetup()
        {
            InitializeComponent();
            load_couriers();
            txtFileNameProduct.Text = "Addressing_guide_with_can_do_delivery.csv";  //JNT ADDRESS
            txtAddressFlash.Text = "FlashServiceAreaManagement.csv";  //Flash ADDRESS

            JNTAddress = Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text);
            FlashAddress = Csv_Controller.GetDataTableFromCSVFile(txtAddressFlash.Text);

            int numberofitems = JNTAddress.Rows.Count + FlashAddress.Rows.Count;
            pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
            lblTotalNumberOfItems.Text = numberofitems.ToString();

            Csv_Controller.dataTableJntAddress = Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text);
            Csv_Controller.dataTableFlashAddress = Csv_Controller.GetDataTableFromCSVFile(txtAddressFlash.Text);

            rdbJandT.IsChecked = true;
            rdbJandTCustomer.IsChecked = true;

        }
        db_queries queries = new db_queries();
        private void load_couriers()
        {
            List<String> couriers = new List<String>();
            couriers.Add("JNT");
            couriers.Add("Flash");
            //cmbCourier.ItemsSource = couriers;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnImportAddress.IsEnabled = false;
            workerImportAddress = new BackgroundWorker();
            workerImportAddress.WorkerReportsProgress = true;

            workerImportAddress.DoWork += WorkerImportRegion_DoWork;
            workerImportAddress.RunWorkerCompleted += WorkerImportRegion_RunWorkerCompleted;

            workerImportAddress.RunWorkerAsync();
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
        
        }
        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cmbCityJnt.SelectedIndex = -1;
            cmbBarangayJnt.ItemsSource = null;

            queries.city(cmbCityJnt, cmbProvinceJnt.Text);
        }

        private void cmbCity_DropDownClosed(object sender, EventArgs e)
        {
            cmbBarangayJnt.SelectedIndex = -1;
            cmbBarangayJnt.ItemsSource = null;

            queries.baranggay(cmbBarangayJnt, cmbCityJnt.Text);
        }

        private void WorkerImportRegion_DoWork(object sender, DoWorkEventArgs e)
        {
            Csv_Controller.ImportAddress(lblImportedProducts, pbBarProduct);
        }
        private void WorkerImportRegion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Import address successfully", "Success");
            btnImportAddress.IsEnabled = true;
            Csv_Controller.ConfirmedToImport = true;
            queries.province(cmbProvinceJnt);
        }
        private void btnBrowseAddress_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFileNameProduct.Text = openFileDialog.FileName;
                Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                int numberofitems = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName).Rows.Count;
                pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
                lblTotalNumberOfItems.Text = numberofitems.ToString();
                Csv_Controller.dataTablebulkOrder = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
            }
        }

        private void btnImportAddress_Click(object sender, RoutedEventArgs e)
        {
            btnImportAddress.IsEnabled = false;
            workerImportAddress = new BackgroundWorker();
            workerImportAddress.WorkerReportsProgress = true;

            workerImportAddress.DoWork += WorkerImportRegion_DoWork;
            workerImportAddress.RunWorkerCompleted += WorkerImportRegion_RunWorkerCompleted;

            workerImportAddress.RunWorkerAsync();
        }

        private void txtAddress_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txtAddress_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            // Check if the entered text contains symbols
            if (HasSymbols(e.Text))
            {
                e.Handled = true; // Ignore the input
            }
        }
        private bool HasSymbols(string text)
        {
            foreach (char c in text)
            {
                // Check if the character is not alphanumeric, space, ",", ".", or "-"
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != ',' && c != '.' && c != '-')
                {
                    return true;
                }
            }
            return false;
        }
        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }
        private void rdbJandT_Checked(object sender, RoutedEventArgs e)
        {
            CheckedRadio(rdbJandT.Content.ToString());
        }

        private void rdbFlash_Checked(object sender, RoutedEventArgs e)
        {
            CheckedRadio(rdbFlash.Content.ToString());
        }

        private void btnSaveShop_Click(object sender, RoutedEventArgs e)
        {
            if(rdbJandT.IsChecked == true)
            {
                queries.insert_sender("0", txtPagename, txtPhone, cmbProvinceJnt, cmbCityJnt, cmbBarangayJnt, txtAddress, "", 1);
            }
            else
            {
                //FLASH
                queries.insert_sender("0", txtPagename, txtPhone, cmbProvinceFlash, cmbCityFlash, cmbBarangayFlash, txtAddress, cmbPostalCodeFlash.Text, 2);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(txtCustomerID, txtEccompanyId))
            {
                MessageBox.Show("Please complete all required fields on customer information.");
                return;
            }
            else
            {
                if(rdbFlash.IsChecked == true)
                {
                    queries.api_credentials(rdbFlashCustomer, "41de95733630f05b050d00c308f13d459a92d64595bac9a29d711bce191dfb2e", "", txtCustomerID);
                    MessageBox.Show("Information Setup completed");
                    MainWindow main = new MainWindow();
                    main.Show();
                }
                else
                {
                    //J&T
                    queries.api_credentials(rdbJandT, "03bf07bf1b172b13efb6259f44190ff3", txtEccompanyId.Text, txtCustomerID);
                    MessageBox.Show("Information Setup completed");
                    MainWindow main = new MainWindow();
                    main.Show();
                }
            }
            this.Close();
        }
    }
}
