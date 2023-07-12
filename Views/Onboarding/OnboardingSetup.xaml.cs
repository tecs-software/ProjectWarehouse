using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static WarehouseManagement.Models.Address;

namespace WarehouseManagement.Views.Onboarding
{
    /// <summary>
    /// Interaction logic for OnboardingSetup.xaml
    /// </summary>
    public partial class OnboardingSetup : Window
    {
        BackgroundWorker workerImportAddress;
        public OnboardingSetup()
        {
            InitializeComponent();
            txtId.Text = "0";
            load_couriers();
            txtFileNameProduct.Text = "Addressing_guide_with_can_do_delivery.csv";
            Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text);
            int numberofitems = Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text).Rows.Count;
            pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
            lblTotalNumberOfItems.Text = numberofitems.ToString();
            Csv_Controller.dataTableAddress = Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text);
        }
        db_queries queries = new db_queries();
        private void load_couriers()
        {
            List<String> couriers = new List<String>();
            couriers.Add("J&T");

            cmbCourier.ItemsSource = couriers;
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
            
            if(Util.IsAnyTextBoxEmpty(txtAddress,txtPagename,txtPhone) || Util.IsAnyComboBoxItemEmpty(cmbProvince) || Util.IsAnyComboBoxItemEmpty(cmbCity) || Util.IsAnyComboBoxItemEmpty(cmbBarangay))
            {
                MessageBox.Show("Please complete all required fields on sender information.");
                return;
            }
            else
            {
                if (Util.IsAnyTextBoxEmpty(txtCustomerID, txtEccompanyId) || Util.IsAnyComboBoxItemEmpty(cmbCourier))
                {
                    MessageBox.Show("Please complete all required fields on customer information.");
                    return;
                }
                else
                {
                    if (queries.insert_sender(txtId,txtPagename, txtPhone, cmbProvince, cmbCity, cmbBarangay, txtAddress))
                    {
                        queries.api_credentials(cmbCourier, "8049bdb499fc06b6fde3e476a87987ef", txtEccompanyId, txtCustomerID);
                        MessageBox.Show("Information Setup completed");
                        MainWindow main = new MainWindow();
                        main.Show();
                    }
                    else
                    {
                        MessageBox.Show("Debug");
                        return;
                    }
                }
            }
            this.Close();
        }
        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cmbCity.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.city(cmbCity, cmbProvince.Text);
        }

        private void cmbCity_DropDownClosed(object sender, EventArgs e)
        {
            cmbBarangay.SelectedIndex = -1;
            cmbBarangay.ItemsSource = null;

            queries.baranggay(cmbBarangay, cmbCity.Text);
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
            queries.province(cmbProvince);
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
                Csv_Controller.dataTableAddress = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
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
    }
}
