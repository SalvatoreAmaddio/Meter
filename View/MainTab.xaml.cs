using Testing.Controller;
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
using Meter.View;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for MainTab.xaml
    /// </summary>
    public partial class MainTab : Window {

        public MainTab() {
            InitializeComponent();
            WindowTracker.MainTab = this;
            WindowTracker.TenantList = TenantList;
            WindowTracker.AddressList = Addresses;
            WindowTracker.InvoiceList=InvoiceList;
            WindowTracker.CityList = CityList;
            WindowTracker.IncomeStatement = IncomeStatement;
        }

        private void OpenPriceForm(object sender, RoutedEventArgs e)
        {
            PriceForm priceForm = new ();
            priceForm.ShowDialog(); 
        }

        private void OpenSoftwareInfo(object sender, RoutedEventArgs e)
        {
            SoftwareInfo softwareInfo = new();
            softwareInfo.ShowDialog();
        }

        private void OpenEmailPassword(object sender, RoutedEventArgs e)
        {
            EmailPasswordManager emailPasswordManager = new();
            emailPasswordManager.ShowDialog();
        }

        private void OpenEmailSetting(object sender, RoutedEventArgs e)
        {
            EmailSetting emailSetting = new();
            emailSetting.ShowDialog();  
        }

    }
}
