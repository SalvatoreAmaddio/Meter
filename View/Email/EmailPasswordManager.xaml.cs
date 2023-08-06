using Meter.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for EmailPasswordManager.xaml
    /// </summary>
    public partial class EmailPasswordManager : Window
    {
        public EmailController Controller { get; }

        public EmailPasswordManager()
        {
            InitializeComponent();
            Controller = (EmailController)DataContext;
            pwd.Password = Controller.Record.Pwd;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Controller.Record.Pwd = pwd.Password;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Controller.Record.IsDirty) {
                Controller.Save();
            }
        }
    }
}
