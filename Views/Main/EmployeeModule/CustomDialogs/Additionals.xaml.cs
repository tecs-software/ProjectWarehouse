using MaterialDesignThemes.Wpf;
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
using WarehouseManagement.Helpers;

namespace WarehouseManagement.Views.Main.EmployeeModule.CustomDialogs
{
    /// <summary>
    /// Interaction logic for Additionals.xaml
    /// </summary>
    public partial class Additionals : Window
    {
        public string? employeeName { get; set; }
        public string? amount { get; set; }
        public string? userId { get; set; }
        public decimal? hoursWorked { get; set; }

        public string? description { get; set; }

        private string? type;

        public Additionals()
        {
            InitializeComponent();
        }

        public void SetData(string type, string user_id ,string name, decimal hours_worked)
        {
            
            employeeName = name;
            lbName.Text = $"Employee: {employeeName}";
            this.userId = user_id;
            this.type = type;

            switch (type.ToLower())
            {
                case "overtime":
                    
                    tbDescription.Visibility = Visibility.Collapsed;
                    lblTitle.Text = "Add Overtime";
                    HintAssist.SetHint(tbAmount, "Overtime (hour)");
                    this.hoursWorked = hours_worked;
                    break;
                case "commission":
                    lblTitle.Text = "Add Commission";
                    HintAssist.SetHint(tbAmount, "Commission Amount");
                    break;
                case "deduction":
                    lblTitle.Text = "Add Deduction";
                    HintAssist.SetHint(tbAmount, "Deduction Amount");
                    break;
                case "reimbursement":
                    lblTitle.Text = "Add Reimbursement";
                    HintAssist.SetHint(tbAmount, "Reimbursement Amount");
                    break;
            }

            this.SizeToContent = SizeToContent.Height;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            switch (type.ToLower())
            {
                case "overtime":
                    if (Convert.ToDecimal(tbAmount.Text) >= hoursWorked)
                    {
                        MessageBox.Show("Invalid Overtime");
                        return;
                    }
                    break;
                case "commission":
                case "deduction":
                case "reimbursement":
                    description = tbDescription.Text;
                    break;
            }

            
            amount = tbAmount.Text;
            DialogResult = true;
        }

        private void DecimalValidation(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Decimal(sender, e);
        }
    }
}
