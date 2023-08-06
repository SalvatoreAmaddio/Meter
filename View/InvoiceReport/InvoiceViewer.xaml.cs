using Meter.Model;
using System.Windows;
using Testing.Controller;
using Testing.Model;
using Testing.Report;

namespace Testing.View
{
    /// <summary>
    /// Interaction logic for InvoiceViewer.xaml
    /// </summary>
    public partial class InvoiceViewer : Window
    {
        
        public InvoiceViewer(InvoiceStream invoiceStream)
        {
            InitializeComponent();
            WindowTracker.InvoiceViewer = this;
            ReportPage reportPage = new(new InvoicePaper(invoiceStream));
            ReportVier.EmailTo = invoiceStream.Invoice.Tenant.Email;
            ReportVier.FixedDoc.Pages.Add(reportPage.GetPage());
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            WindowTracker.InvoiceViewer = null;
        }
    }
}
