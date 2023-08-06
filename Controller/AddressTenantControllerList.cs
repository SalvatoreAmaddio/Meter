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
using System.Windows.Media.Media3D;
using Testing.Controller;
using Testing.RecordSource;

namespace Meter.Controller {

    public class AddressTenantControllerList : AbstractListDataController<TenantAddress> {
        public event EventHandler<NotifyTenantFormEventArgs>? NotifyTenantForm;
        public ReadingListController ReadingListController { get; } = new();

        public AddressTenantControllerList() =>
            RecordSource.ReplaceRecords(MainDB.TenantAddressTable.RecordSource);

        public AddressTenantControllerList(Tenant tenant) {
            ReadingListController.RecordSource.ReplaceRecords(MainDB.ReadingTable.RecordSource);
            Requery(tenant);
            RecordTracker.AllowNewRecord = false;
            RecordSource.OnDeleteCascadeEvent += RecordSource_OnDeleteCascadeEvent;
            RecordTracker.OnNextRecordEvt += RecordTracker_OnNextRecordEvt;
        }

        private void RecordTracker_OnNextRecordEvt(object? sender, OnNextRecordEvtArgs e)
        {
            SelectedRecord.IsDirty = false;
            Refilter();
        }

        public void Requery(Tenant tenant)
        {
            RecordSource.ReplaceRecords(MainDB.TenantAddressTable.RecordSource.Where(s => s.Tenant.Equals(tenant)).OrderByDescending(s => s.MovedIn).OrderByDescending(s => s.TenantAddressID));
            SelectedRecord = (RecordSource.Any()) ? RecordSource.FirstOrDefault() : new();
            SelectedRecord.IsDirty = false;
        }

        protected override void DeleteRecord(TenantAddress record)
        {
            if (ConfirmDialog() == MessageBoxResult.No) return;
            MainDB.TenantAddressTable.Delete(SelectedRecord);
            RecordTracker=new RecordTracker { RecordCount=RecordSource.Count,CurrentRecord=1,AllowNewRecord=false};

            if (RecordTracker.EOF)
            {
                SelectedRecord = RecordSource.LastOrDefault();
                Refilter();
                return;
            }

            if (RecordTracker.BOF) {
                SelectedRecord = RecordSource.FirstOrDefault();
                Refilter();
                return;
            }

            if (RecordTracker.CanGoPrevious) {
                PreviousRecord();
                return;
            }

            if (RecordTracker.CanGoNext) {
                NextRecord();
                return;
            }
        SelectedRecord = new();
        NotifyTenantForm?.Invoke(this, new NotifyTenantFormEventArgs(SelectedRecord, Operations.DELETE));
        }

        private void RecordSource_OnDeleteCascadeEvent(object? sender, Testing.RecordSource.OnCascadeQueryEventArgs<TenantAddress> e)
        {
            var source = ReadingListController.RecordSource.Where(s => s.TenantAddress.TenantAddressID == e.Record.TenantAddressID);
            ReadingListController.RecordSource.RemoveRecords(source);
        }
        
        private void Refilter()
        {
            NotifyTenantForm?.Invoke(this, new NotifyTenantFormEventArgs(SelectedRecord, Operations.PREVIOUS, RecordTracker.CurrentRecord));
            ReadingFilter readingFilter = new();
            readingFilter.TenantAddressID = SelectedRecord.TenantAddressID;
            ReadingListController.Filter = readingFilter;
        }

        protected override void SaveRecord(TenantAddress record)=>MainDB.TenantAddressTable.Update(record);

        protected override void OpenRecord(TenantAddress record)
        {
            throw new NotImplementedException();
        }

        protected override void OpenNewRecord()
        {
            throw new NotImplementedException();
        }

        public override void SetRecordTrackerSource()
        {
        }

    }

    public class NotifyTenantFormEventArgs : EventArgs { 
        public int TenantAddressID { get; set; }
        private Operations Operation { get; set; }
        private int CurrentRecord { get; set; }
        public TenantAddress TenantAddress { get; set; }

        public NotifyTenantFormEventArgs(TenantAddress tenantAddress, Operations operation) {
            TenantAddress = tenantAddress;
            Operation = operation;
        } 
        
        public NotifyTenantFormEventArgs(TenantAddress tenantAddress, Operations operation, int currentrecord) : this(tenantAddress, operation) 
        {
            CurrentRecord = currentrecord;
        }

        public bool IsDelete() => Operation == Operations.DELETE;
        public bool IsNext()=>Operation == Operations.NEXT;
        public bool IsPrevious() => Operation == Operations.PREVIOUS;

        public string CurrentAddress() =>(CurrentRecord == 1) ? "Current Address" : "Previous Address";
        public string CurrentAddressReadings() => (CurrentRecord == 1) ? "Address' Readings" : "Previous Address's Readings";

    }

    public enum Operations{ 
        DELETE,
        PREVIOUS,
        NEXT
    }
}