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

namespace WarehouseManagement.Views.Onboarding
{
    /// <summary>
    /// Interaction logic for OnboardingSetup.xaml
    /// </summary>
    public partial class OnboardingSetup : Window
    {
        public OnboardingSetup()
        {
            InitializeComponent();
            load_couriers();
        }
        private void load_couriers()
        {
            List<String> couriers = new List<String>();
            couriers.Add("JnT");

            cbCourier.ItemsSource = couriers;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
    }
}
