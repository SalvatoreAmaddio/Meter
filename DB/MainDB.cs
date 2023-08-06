using Testing.Controller;
using Testing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.RecordSource;
using Meter.Model;
using System.Windows.Media.Media3D;

namespace Testing.DB
{
    public static class MainDB {

        public static DBLite<Tenant> TenantTable { get; set; } = new();
        public static DBLite<Address> AddressTable { get; set; } = new();
        public static DBLite<City> CityTable { get; set; } = new();
        public static DBLite<TenantAddress> TenantAddressTable { get; set; } = new();
        public static DBLite<Reading> ReadingTable { get; set; } = new();
        public static DBLite<Invoice> InvoiceTable { get; set; } = new();
        public static DBLite<InvoiceReading> InvoiceReading { get; set; } = new();
        public static DBLite<Price> PriceTable { get; set; } = new();
        public static DBLite<EmailModel> Email { get; set; } = new();

        public static void SetEvents() {
           TenantTable.RecordSource.OnAddingCascadeEvent += Tenant_OnAddingCascadeEvt;
           TenantTable.RecordSource.OnDeleteCascadeEvent += Tenant_OnDeleteCascadeEvt;            

           TenantAddressTable.RecordSource.OnDeleteCascadeEvent += TenantAddress_OnDeleteCascadeEvt; ;
           TenantAddressTable.RecordSource.OnAddingCascadeEvent += TenantAddress_OnAddingCascadeEvt;

           ReadingTable.RecordSource.OnDeleteCascadeEvent += Reading_OnDeleteCascadeEvt;

           InvoiceReading.RecordSource.OnDeleteCascadeEvent += InvoiceReading_OnDeleteCascadeEvt;            
           
            InvoiceTable.RecordSource.OnAddingCascadeEvent += Invoice_OnAddingCascadeEvt;
            InvoiceTable.RecordSource.OnDeleteCascadeEvent += Invoice_OnDeleteCascadeEvt;

           AddressTable.RecordSource.OnDeleteCascadeEvent += Address_OnDeleteCascadeEvt;
           AddressTable.RecordSource.OnAddingCascadeEvent += Address_OnAddingCascadeEvt; 

           CityTable.RecordSource.OnDeleteCascadeEvent += City_OnDeleteCascadeEvt;
           CityTable.RecordSource.OnAddingCascadeEvent += City_OnAddingCascadeEvt;
        }

        #region Address Cascade
        private static void Address_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<Address> e)
        {
            var records = MainDB.TenantAddressTable.RecordSource.Where(s => s.Address.AddressID == e.Record.AddressID);
            MainDB.TenantAddressTable.RecordSource.RemoveRecords(records);
            WindowTracker.AddressList?.Controller.RecordSource.RemoveRecord(e.Record);
        }

        private static void Address_OnAddingCascadeEvt(object? sender, OnCascadeQueryEventArgs<Address> e)
        {
            if (e.Crud.Equals(Crud.INSERT))
            {
                WindowTracker.AddressList?.Controller.RecordSource.AddRecord(e.Record);
            }
            else {
                WindowTracker.AddressList?.Controller.RecordSource.ReplaceRecord(e.Record);
            }
        }
        #endregion

        #region TenantCascade
        private static void Tenant_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<Tenant> e)
        {
            WindowTracker.TenantList?.Controller?.RecordSource.RemoveRecord(e.Record);
            var records = MainDB.TenantAddressTable.RecordSource.Where(s => s.Tenant.TenantID == e.Record.TenantID);
            MainDB.TenantAddressTable.RecordSource.RemoveRecords(records);
        }

        private static void Tenant_OnAddingCascadeEvt(object? sender, OnCascadeQueryEventArgs<Tenant> e)
        {
            WindowTracker.TenantList?.Controller?.RecordSource.AddRecord(e.Record);
            WindowTracker.TenantList?.Controller?.OrderBy();
        }
        #endregion

        #region invoice cascade
        private static void Invoice_OnAddingCascadeEvt(object? sender, OnCascadeQueryEventArgs<Invoice> e) {
            
            if (e.Crud.Equals(Crud.INSERT)) {
                WindowTracker.InvoiceList?.Controller.RecordSource.AddRecord(e.Record);
            }
            else
            {
                WindowTracker.InvoiceList?.Controller.RecordSource.ReplaceRecord(e.Record);
            }

            WindowTracker.InvoiceList?.Controller.RefreshOverdueTracker(e.Record);

            if (WindowTracker.InvoiceList != null)
            {
                WindowTracker.InvoiceList.Controller.Order = new();
                WindowTracker.InvoiceList.Controller.SelectedRecord = e.Record;
            }

            WindowTracker.IncomeStatement?.Controller?.Refresh();
        }

        private static void Invoice_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<Invoice> e)
        {
            WindowTracker.InvoiceList?.Controller.RecordSource.RemoveRecord(e.Record);
            WindowTracker.IncomeStatement?.Controller?.Refresh();
        }

        #endregion

        #region TenantAddress cascade
        public static void TenantAddress_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<TenantAddress> e)
        {
            var records = ReadingTable.RecordSource.Where(s => s.TenantAddress.TenantAddressID == e.Record.TenantAddressID);
            ReadingTable.RecordSource.RemoveRecords(records);
            WindowTracker.TenantForm?.Controller.AddressTenantControllerList.RecordSource.RemoveRecord(e.Record);
        }

        private static void TenantAddress_OnAddingCascadeEvt(object? sender, OnCascadeQueryEventArgs<TenantAddress> e)
        {
            var controller=WindowTracker.TenantForm?.Controller;

            if (e.Crud.Equals(Crud.UPDATE))
            {
                if (controller != null)
                {
                    controller?.AddressTenantControllerList.RecordSource.ReplaceRecord(e.Record);
                    return;
                }
                return;
            }

            if (e.Crud.Equals(Crud.INSERT)) {
                if (controller != null)
                {
                    controller?.AddressTenantControllerList.Requery(controller.Record);
                    controller.Record.TenantAddress = controller.AddressTenantControllerList.SelectedRecord;
                    controller.EnableMeterNumber = TenantAddress.GetLastAddress(controller.Record);
                    controller.AddressTenantControllerList.ReadingListController.ResetFilter(controller.Record.TenantAddress);
                    controller.Record.IsDirty = false;
                }
            }

            WindowTracker.AssignAddress?.Close();
        }
        #endregion

        #region invoice reading cascade
        private static void InvoiceReading_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<InvoiceReading> e)
        {
            var record = InvoiceTable.RecordSource.Where(s => s.InvoiceID == e.Record.Invoice.InvoiceID).FirstOrDefault();
            InvoiceTable.RecordSource.RemoveRecord(record);
            WindowTracker.InvoiceList?.Controller.RecordSource.RemoveRecord(record);
        }

        #endregion

        #region reading cascade
        private static void Reading_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<Reading> e)
        {
            var records = InvoiceReading.RecordSource.Where(s => s.Reading.ReadingID == e.Record.ReadingID);
            InvoiceReading.RecordSource.RemoveRecords(records);
        }
        #endregion

        #region city cascade
        private static void City_OnAddingCascadeEvt(object? sender, OnCascadeQueryEventArgs<City> e)
        {
            WindowTracker.CityList?.Controller.RecordSource.AddRecord(e.Record);
        }

        private static void City_OnDeleteCascadeEvt(object? sender, OnCascadeQueryEventArgs<City> e)
        {
            WindowTracker.CityList?.Controller.RecordSource.RemoveRecord(e.Record);
        }
        #endregion

        public static async Task Fetch() {
            await Task.WhenAll(
                Task.Run(() => Email.Select()),
                Task.Run(() => CityTable.Select()),
                Task.Run(() => PriceTable.Select()),
                Task.Run(() => TenantTable.Select()),
                Task.Run(() => TenantAddressTable.Select()),
                Task.Run(() => ReadingTable.Select()),
                Task.Run(() => AddressTable.Select()),
                Task.Run(() => InvoiceTable.Select()),
                Task.Run(() => InvoiceReading.Select())
                ) ;            
        }

    }
}
