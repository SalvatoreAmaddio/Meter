using Testing.Controller;
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
using System.Windows.Shapes;
using Meter.Controller;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for AssignAddress.xaml
    /// </summary>
    public partial class AssignAddress : Window {
       
        public AddressControllerList Controller { get; }

        public AssignAddress() {
            InitializeComponent();
            Controller = (AddressControllerList)DataContext;
            Title = $"Assign Address to {WindowTracker.TenantForm.Controller.Record.FullName}";
            WindowTracker.AssignAddress = this;
        }

    }
}
