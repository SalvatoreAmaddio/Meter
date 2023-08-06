using Meter.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Testing.Controller;
using Testing.DB;
using Meter.Model;
using Testing.Model;

namespace Testing.View
{
    /// <summary>
    /// Interaction logic for InvoiceForm.xaml
    /// </summary>
    public partial class InvoiceForm : Window
    {
        public InvoiceFormController Controller { get; set; }

        public InvoiceForm(Invoice record) {
            Controller = new(record);
            InitializeComponent();
            DataContext = Controller;
            WindowTracker.InvoiceForm = this;
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Controller.Record.IsDirty) return;
            if (!Controller.Record.CanUpdate()) {
                MessageBox.Show("Please, fill all mandatory fields","INPUT ERROR");
                e.Cancel=true;
                return;
            }

            await Task.Delay(1000);
            if (Controller.Record.IsDirty) Controller.Save();

            WindowTracker.InvoiceForm = null;

        }
    }
}
