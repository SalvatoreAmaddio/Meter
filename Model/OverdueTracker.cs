using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.Controller;
using Testing.DB;
using Testing.Model;
using Testing.RecordSource;

namespace Meter.Model {

    public class OverdueTracker : AbstractNotifier {
        double _overdue;
        public double Overdue { get => _overdue; set => Set<Double>(ref value, ref _overdue, nameof(Overdue)); }
        public Visibility ShowOverdue { get => (Overdue > 0) ? Visibility.Visible : Visibility.Collapsed; }

        public OverdueTracker()
        {
            Overdue = 0;
            AfterPropChangedEvt += OverdueTracker_AfterPropChangedEvt;
        }

        private void OverdueTracker_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(Overdue))) NotifyView(nameof(ShowOverdue));            
        }

        public OverdueTracker(Invoice invoice, bool First) {

            var source = MainDB.InvoiceTable
                                   .RecordSource
                                   .Where(s => s.Tenant.Equals(invoice.Tenant)
                                          && (!s.FullyPaid)
                                          && s.PaymentDate < invoice.PaymentDate
                                          );

            var c = source.Count();
            AfterPropChangedEvt += OverdueTracker_AfterPropChangedEvt;
            var lastInvoice = (First) ? source.FirstOrDefault() : source.LastOrDefault();
            Overdue = (lastInvoice != null) ? lastInvoice.AmountDue : 0;
        }

    }
}
