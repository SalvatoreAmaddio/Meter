using Meter.Controller;
using Meter.View.Templates;
using System.Windows;
using System.Windows.Controls;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for AddressList.xaml
    /// </summary>
    public partial class AddressList : Page {

        public AddressControllerList Controller { get; set; }
        
        public AddressList()
        {
            InitializeComponent();
            Controller = (AddressControllerList)DataContext;
        }

    }
}
