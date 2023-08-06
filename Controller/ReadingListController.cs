using Testing.Controller.Interfaces;
using Testing.DB;
using Testing.Model;
using Testing.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.Controller;
using Testing.RecordSource;

namespace Meter.Controller
{
    public class ReadingListController : AbstractListDataController<Reading>
    {
        ReadingFilter _filter = null!;
        public ReadingFilter Filter { get => _filter; set => Set(ref value, ref _filter, nameof(Filter)); }
        DateTime? _datefrom;
        DateTime? _dateto;

        public DateTime? DateFrom { get=>_datefrom; set=>Set<DateTime?>(ref value,ref _datefrom,nameof(DateFrom)); }
        public DateTime? DateTo { get => _dateto; set=>Set<DateTime?>(ref value,ref _dateto,nameof(DateTo)); }

        #region constructors
        public ReadingListController() {
            Filter = new();
            RecordSource.ReplaceRecords(MainDB.ReadingTable.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
            AfterPropChangedEvt += ReadingListController_AfterPropChangedEvt;
        }

        public ReadingListController(RecordSource<Reading> readings)
        {
            Filter = new();
            RecordSource.ReplaceRecords(readings);
            SelectedRecord = RecordSource.FirstOrDefault();
        }
        #endregion

        private void ReadingListController_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            bool IsDateFrom= e.PropIs(nameof(DateFrom));
            bool IsDateTo = e.PropIs(nameof(DateTo));

            if (IsDateFrom || IsDateTo) {
                var date=e.GetValue<DateTime?>();
                if (IsDateFrom) ReadingFilter.DateFrom= date;
                if (IsDateTo) ReadingFilter.DateTo = date;
                Filter = new(Filter.TenantAddressID, false);
            }
        }

        #region abstract definitions
        protected override void OpenNewRecord()
        {
            ReadingForm readingForm = new();
            readingForm.ShowDialog();
        }

        private bool AlreadyInvoiced(string text) {
            var range = MainDB.InvoiceReading.RecordSource.Where(s => s.Reading.ReadingID == SelectedRecord.ReadingID);
            if (range.Any())
            {
                MessageBox.Show($"This reading has been invoiced and cannot be {text}", "Confirm");
                return true;
            }
            return false;
        }

        protected override void DeleteRecord(Reading record)
        {
            SelectedRecord = record;

            if (AlreadyInvoiced("deleted")) return;
            if (ConfirmDialog() == MessageBoxResult.Yes)
            {
                MainDB.ReadingTable.Delete(SelectedRecord);
                RecordSource.RemoveRecord(SelectedRecord);
                ResetFilter();
            }
        }

        protected override void OpenRecord(Reading record)
        {
            throw new NotImplementedException();
        }

        protected override void SaveRecord(Reading record)
        {
            if (AlreadyInvoiced("changed")) return;
            MainDB.ReadingTable.Update(record);
            ResetFilter();
            SelectedRecord = record;
        }

        #endregion

        public override void SetRecordTrackerSource()
        {
        }

        public void ResetFilter(TenantAddress tenantAddress) {
            if (tenantAddress == null) {
                Filter = new();
                return;
            }
            Filter = new(Filter.TenantAddressID, Filter.IgnoreDate);
        }

        public void ResetFilter() =>Filter = new(Filter.TenantAddressID, Filter.IgnoreDate);
    }

    #region ReadingFilterObject
    public class ReadingFilter : IFilterList {
        public Int64 TenantAddressID { get; set; }
        public bool IgnoreDate { get; set; }
        public IRecordSource Obj { get; set; } = new Reading();
        public string? Search { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private static DateTime Today= DateTime.Today;
        private static int Days { get=> DateTime.DaysInMonth(Today.Year, Today.Month); }
        public static DateTime? DateFrom { get; set; } = new DateTime(Today.Year, Today.Month, 1);
        public static DateTime? DateTo { get; set; }= new DateTime(Today.Year, Today.Month, Days);
        public override string? ToString()=>TenantAddressID.ToString();

        public ReadingFilter(Int64 tenantaddressID=0,bool ignoredate=true) {
            TenantAddressID = tenantaddressID;
            IgnoreDate=ignoredate;
        }
    }
    #endregion

}
