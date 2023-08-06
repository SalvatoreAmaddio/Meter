using Testing.DB;
using Testing.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller;
using Testing.RecordSource;
using Testing.Controller.Interfaces;
using Testing.View;
using System.Windows.Input;
using MvvmHelpers.Commands;
using Meter.Model;
using System.Windows;

namespace Meter.Controller
{
    public class InvoiceFormController : AbstractDataController<Invoice>, IRecordSource {

        public RecordSource<InvoiceReading> Readings { get; set; } = new();

        public ICommand OpenInvoiceCMD { get; }

        InvoiceReadingFilter _filter=null!;
        public InvoiceReadingFilter Filter { get => _filter; set => Set<InvoiceReadingFilter>(ref value, ref _filter,nameof(Filter)); }

        public InvoiceFormController() {
            Readings.ReplaceRecords(MainDB.InvoiceReading.RecordSource);
            Filter = new(Record.InvoiceID);
            OpenInvoiceCMD=new Command(OpenInvoice);   
            AfterPropChangedEvt += InvoiceFormController_AfterPropChangedEvt;
        }

        public InvoiceFormController(Invoice invoice)
        {
            Record=invoice;
            Record.Tenant.TenantAddress=TenantAddress.GetInvoicedAddress(Record);
            Record.SetUpOverdueTracker();
            Readings.ReplaceRecords(MainDB.InvoiceReading.RecordSource);
            Filter = new(Record.InvoiceID);
            OpenInvoiceCMD = new Command(OpenInvoice);
            AfterPropChangedEvt += InvoiceFormController_AfterPropChangedEvt;
        }

        private void InvoiceFormController_AfterPropChangedEvt(object? sender, NotifierArgs e) {
            if (e.PropIs(nameof(Record))) Filter = new(Record.InvoiceID);
        }

        protected override void DeleteRecord(Invoice record)
        {
        }

        private void OpenInvoice() =>Invoice.OpenInvoice(Record);

        protected override void SaveRecord(Invoice record)=>MainDB.InvoiceTable.Update(record);

        public string CompleteComboBoxSelectedItem()=>string.Empty;

        public bool FilterRecordSource(IFilterList filter)=>true;

        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)
        {
            return (RecordSource<Invoice>)ItemSource;
        }

        protected override void NextRecord()
        {
            throw new NotImplementedException();
        }

        protected override void PreviousRecord()
        {
            throw new NotImplementedException();
        }

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            throw new NotImplementedException();
        }

        public override void SetRecordTrackerSource()
        {
        }

    }

    #region InvoiceReadingFilterObject
    public class InvoiceReadingFilter : IFilterList
    {
        public string? Search { get; set; }
        public IRecordSource Obj { get; set; }
        public Int64 InvoiceID { get; set; }

        public InvoiceReadingFilter(Int64 val)
        {
            InvoiceID = val;
            Obj = new InvoiceReading();
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
}
