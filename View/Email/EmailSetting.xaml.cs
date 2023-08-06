using Meter.Controller;
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
using Testing.DB;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for EmailSetting.xaml
    /// </summary>
    public partial class EmailSetting : Window
    {
        public EmailController Controller { get; }

        public EmailSetting()
        {
            InitializeComponent();
            Controller = (EmailController)DataContext;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!Controller.Record.IsDirty) return;
            Controller.Save();
        }
    }
}
