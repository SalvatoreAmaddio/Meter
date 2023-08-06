using Meter.Controller;
using System.Windows.Controls;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for InvoiceList.xaml
    /// </summary>
    public partial class InvoiceList : Page
    {
        public InvoiceListController Controller { get; set; }

        public InvoiceList()
        {
            InitializeComponent();
            Controller = (InvoiceListController)DataContext;
        }
    }
}
