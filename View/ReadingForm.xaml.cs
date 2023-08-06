using Meter.Controller;
using System;
using System.Windows;
using Testing.Controller;

namespace Testing.View
{
    /// <summary>
    /// Interaction logic for ReadingForm.xaml
    /// </summary>
    public partial class ReadingForm : Window {

        public ReadingFormController Controller { get; set; }   

        public ReadingForm()
        {
            InitializeComponent();
            Controller = (ReadingFormController)DataContext;
            Controller.Record.TenantAddress = WindowTracker.TenantForm.Controller.Record.TenantAddress;
            Controller.Record.IsDirty = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!Controller.Record.IsDirty) return;
            Controller.Save();
        }
    }
}
