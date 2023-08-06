using Testing.DB;
using Testing.Model;
using Testing.View;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Testing.Controller;
using Testing.Controller.Interfaces;
using Testing.RecordSource;
using Meter.Controller.Interfaces;
using Meter.Model;

namespace Meter.Controller
{
    public class InvoiceListController : AbstractListDataController<Invoice> {

        public ICommand OpenInvoiceCMD { get; }

        InvoiceFilter _filter = null!;
        public InvoiceFilter Filter { get => _filter; set => Set<InvoiceFilter>(ref value, ref _filter, nameof(Filter)); }

        InvoiceOrder _order=null!;
        public InvoiceOrder Order 
        { 
            get => _order; 
            set => Set<InvoiceOrder>(ref value, ref _order, nameof(Order)); 
        }

        public InvoiceListController() {
            Order = new();
            Filter = new(string.Empty);
            RecordSource.ReplaceRecords(MainDB.InvoiceTable.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
            OpenInvoiceCMD = new Command<Invoice>(OpenInvoice);
            AfterPropChangedEvt += InvoiceListController_AfterPropChangedEvt;
        }

        public void RefreshOverdueTracker(Invoice record)
        {
            SelectedRecord = record;
            var source = MainDB.InvoiceTable.RecordSource.Where(s=>s.Tenant.Equals(SelectedRecord.Tenant)).OrderBy(s=>s.PaymentDate);
            foreach (var invoice in source)
            {
                    invoice.SetUpOverdueTracker();
            }
        }

        private void InvoiceListController_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(Search))) Filter = new(e.GetValue<string?>());
        }

        private void OpenInvoice(Invoice invoice) {
            SelectedRecord= invoice;
            Invoice.OpenInvoice(SelectedRecord);
        }

        public void Refresh()
        {
            RecordSource.ReplaceRange(MainDB.InvoiceTable.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
        }

        protected override void OpenNewRecord()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteRecord(Invoice record)
        {
            SelectedRecord = record;
            if (ConfirmDialog() == System.Windows.MessageBoxResult.No) return;
            var index = RecordSource.IndexOf(SelectedRecord);
            MainDB.InvoiceTable.Delete(SelectedRecord);
            SelectedRecord=RecordSource[index];
        }

        protected override void OpenRecord(Invoice record)
        {
            SelectedRecord= record;
            InvoiceForm invoiceForm = new(SelectedRecord);
            invoiceForm.ShowDialog();
        }

        protected override void SaveRecord(Invoice record)
        {

        }

        public override void SetRecordTrackerSource()
        {
        }

    }

    #region TenatFilterObject
    public class InvoiceFilter : IFilterList
    {
        public string? Search { get; set; }
        public IRecordSource Obj { get; set; }

        public InvoiceFilter(string? val)
        {
            Search = val;
            Obj = new Invoice();
        }

        public override string? ToString() => $"{Search}";

        public override bool Equals(object? obj)
        {
            return obj is TenantFilter filter &&
                   Search == filter.Search;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Search);
        }
    }
    #endregion

    #region InvoiceOrder
    public class InvoiceOrder : IListOrder
    {
        public IRecordSource Obj { get; set; } = new Invoice();

    }
    #endregion

}
