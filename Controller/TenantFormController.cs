using Testing.DB;
using Testing.Model;
using Testing.View;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Testing.Controller;
using Testing.RecordSource;
using System.Security.Policy;
using Meter.Model;
using Meter.View;

namespace Meter.Controller {

    public class TenantFormController : AbstractDataController<Tenant> {

        public ICommand RemoveDateFilterCMD { get; }
        public ICommand AssignAddressCMD { get; }
        public ICommand OpenReadingCMD { get; }
        public ICommand MakeInvoiceCMD { get; }
        public AddressTenantControllerList AddressTenantControllerList { get; private set; }
        public InvoiceListController InvoiceListController { get; } = new();

        bool _enableMeterNumber;
        public bool EnableMeterNumber { get => _enableMeterNumber; set => Set(ref value, ref _enableMeterNumber, nameof(EnableMeterNumber)); }

        string _previouscurrentaddress = "Current Address";
        public string PreviousCurrentAddress { get => _previouscurrentaddress; set => Set(ref value, ref _previouscurrentaddress, nameof(PreviousCurrentAddress)); }

        string _previouscurrentaddressreadings = "Address' Readings";
        public string PreviousCurrentAddressReadings { get => _previouscurrentaddressreadings; set => Set(ref value, ref _previouscurrentaddressreadings, nameof(PreviousCurrentAddressReadings)); }

        public TenantFormController(Tenant tenant)
        {
            RemoveDateFilterCMD = new Command(RemoveDateFilter);
            AssignAddressCMD = new Command(AssignAddress);
            MakeInvoiceCMD = new Command(MakeInvoice);
            OpenReadingCMD = new Command(OpenReading);
            Record = tenant;
            EnableMeterNumber = TenantAddress.GetLastAddress(Record);

            AddressTenantControllerList = new(Record);
            AddressTenantControllerList.ReadingListController.ResetFilter(Record.TenantAddress);

            Record.TenantAddress.IsDirty = false;
            Record.IsDirty = false;

            AddressTenantControllerList.NotifyTenantForm += AddressTenantControllerList_NotifyTenantForm;

            RecordTracker.OnNextRecordEvt += RecordTracker_OnNextRecordEvt;
            RecordTracker.OnNewRecordEvt += RecordTracker_OnNewRecordEvt;
        }

        public override void SetRecordTrackerSource()
        {
            RecordTracker.Source = MainDB.TenantTable.RecordSource;
        }

        private void RecordTracker_OnNextRecordEvt(object? sender, OnNextRecordEvtArgs e)
        {
            EnableMeterNumber = TenantAddress.GetLastAddress(Record);
            AddressTenantControllerList.SelectedRecord = Record.TenantAddress;
            AddressTenantControllerList.ReadingListController.ResetFilter(Record.TenantAddress);
            AddressTenantControllerList.SelectedRecord.IsDirty = false;
            AddressTenantControllerList.Requery(Record);
            PreviousCurrentAddress = "Current Address";
            PreviousCurrentAddressReadings = "Address' Readings";
        }

        private void RecordTracker_OnNewRecordEvt(object? sender, EventArgs e)
        {
            Record.TenantAddress = new();
            Record.IsDirty = false;
            EnableMeterNumber = false;
        }

        #region SaveDeleteCommands
        protected override void SaveRecord(Tenant record)
        {
            Record = record;

            if (Record.TenantID > 0) {
                MainDB.TenantTable.Update(Record);
                return;
            }

                Record.TenantAddress = new();
                MainDB.TenantTable.Insert(Record);
        }

        protected override void DeleteRecord(Tenant record)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CMD

        private void RemoveDateFilter() {
            ReadingFilter filter = new();
            filter.TenantAddressID = AddressTenantControllerList.ReadingListController.Filter.TenantAddressID;
            filter.IgnoreDate = true;
            AddressTenantControllerList.ReadingListController.Filter = filter;
        }

        private void AssignAddress() {
            if (!Record.CanUpdate())
            {
                return;
            }

            Save();
            AssignAddress assignAddress = new();
            assignAddress.ShowDialog();
        }

        private void OpenReading() {
            if (!Record.CanUpdate())
            {
                return;
            }

            Save();

            if (Record.TenantAddress.Address.AddressID == 0)
            {
                MessageBox.Show("Please select an address before adding a reading", "INPUT ERROR");
                return;
            }
            AddressTenantControllerList.ReadingListController.OpenNewRecordCMD.Execute(null);
        }

        private void MakeInvoice()
        {
            if (AddressTenantControllerList.ReadingListController.RecordSource.Count() <= 1)
            {
                MessageBox.Show("You must have at least two readings", "INPUT ERROR");
                return;
            }

            if (!Record.CanUpdate())
            {
                MessageBox.Show("Please fill all mandatory fields before adding a reading", "INPUT ERROR");
                return;
            }

            Save();

            if (Record.TenantAddress.Address.AddressID == 0)
            {
                MessageBox.Show("Please select an address before adding a reading", "INPUT ERROR");
                return;
            }

            AddressTenantControllerList.ReadingListController.RecordSource.Refresh();
            InvoiceListController.OpenRecordCMD.Execute(new Invoice(AddressTenantControllerList.ReadingListController.RecordSource, Record));
        }
        #endregion

        private void AddressTenantControllerList_NotifyTenantForm(object? sender, NotifyTenantFormEventArgs e)
        {
            if (e.IsDelete())
            {
                Record.TenantAddress = e.TenantAddress;
                EnableMeterNumber = TenantAddress.GetLastAddress(Record);
                PreviousCurrentAddress = e.CurrentAddress();
                PreviousCurrentAddressReadings = e.CurrentAddressReadings();
                return;
            }

            Record.TenantAddress = e.TenantAddress;
            EnableMeterNumber = true;
            PreviousCurrentAddress = e.CurrentAddress();
            PreviousCurrentAddressReadings = e.CurrentAddressReadings();
        }

    }
}