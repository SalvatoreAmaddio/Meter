using Meter.Controller;
using System.Windows;

namespace Testing.View
{
    /// <summary>
    /// Interaction logic for CityForm.xaml
    /// </summary>
    public partial class CityForm : Window
    {
        public CityFormController Controller { get; set; }

        public CityForm()
        {
            InitializeComponent();
            Controller = (CityFormController)DataContext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool IsDirty = Controller.Record.IsDirty;

            if (!IsDirty) {
                return;
            }

            if (!Controller.Record.CanUpdate()) {
                return;
            }

            Controller.Save();
        }

    }
}
