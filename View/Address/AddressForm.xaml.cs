using Testing.Controller;
using Testing.DB;
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
using Meter.Utilities;

namespace Testing.View
{
    /// <summary>
    /// Interaction logic for AddressForm.xaml
    /// </summary>
    public partial class AddressForm : Window {

        public AddressFormController Controller { get; set; }

        public AddressForm()
        {
            InitializeComponent();
            Controller = (AddressFormController)DataContext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Controller.Record.IsDirty) return;
            var x = Controller.Record.AddressLine;
            Controller.Record.AddressLine = Sys.EachFirstLetterUpper(ref x);

            if (!Controller.Record.CanUpdate()) {
                e.Cancel = true;
                return;
            }

            Controller.Save();
        }
    }
}
