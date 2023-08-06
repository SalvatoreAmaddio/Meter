using Meter.Controller;
using Testing.DB;
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
using Testing.Customs;
using Testing.Controller;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for TenantForm.xaml
    /// </summary>
    public partial class TenantForm : Window {

        public TenantFormController Controller { get; }

        #region constructors

       

        public TenantForm(Tenant tenant)  {
            InitializeComponent();
            Controller = new(tenant);
            WindowTracker.TenantForm = this;
            DataContext = Controller;
        }

        #endregion


        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

            bool RecordIsDirty = Controller.Record.IsDirty;
            bool SubRecordIsDirty = Controller.Record.TenantAddress.IsDirty;
            
            if (!RecordIsDirty && !SubRecordIsDirty) 
            {
                WindowTracker.TenantForm = null;
                return;
            }

            if (!Controller.Record.CanUpdate()) 
            {
                e.Cancel = true;
                return;
            }

            if (!Controller.AddressTenantControllerList.SelectedRecord.CanUpdate())
            {
                e.Cancel = true;
                return;
            }

            await Task.Delay(1000);
            if (RecordIsDirty) Controller.Save();
            if (SubRecordIsDirty) Controller.AddressTenantControllerList.Save();
            WindowTracker.TenantForm = null;
            WindowTracker.TenantList.Controller.Order = new();
        }
       
    }
}
