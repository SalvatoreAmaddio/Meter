using Testing.Model;
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
using Testing.Report;
using Meter.Model;

namespace Testing.View
{
    /// <summary>
    /// Interaction logic for InvoicePaper.xaml
    /// </summary>
    public partial class InvoicePaper : PaperBase
    {

        public InvoiceStream? Invoice { get; }

        public InvoicePaper()
        {
            InitializeComponent();
          
        }

        public InvoicePaper(InvoiceStream invoiceStream) : this() {
            Invoice = invoiceStream;
            DataContext = Invoice;  
        }

        public override PaperBase ReturnNew()=>new InvoicePaper();
        
    }
}
