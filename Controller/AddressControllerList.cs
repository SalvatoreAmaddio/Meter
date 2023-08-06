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

namespace Meter.Controller
{
    public class AddressControllerList : AbstractListDataController<Address> {

        public ICommand SelectAddressCMD { get;}
        public RecordSource<City> Cities { get; set; } = new();

        AddressFilter _filter = null!;
        public AddressFilter Filter { get => _filter; set => Set<AddressFilter>(ref value, ref _filter, nameof(Filter)); }

        public AddressControllerList() {
            RecordSource.ReplaceRecords(MainDB.AddressTable.RecordSource);
            Cities.ReplaceRange(MainDB.CityTable.RecordSource);
            SelectAddressCMD = new Command<Address>(SelectedAddress);
            SelectedRecord = RecordSource.FirstOrDefault();
            Filter = new(string.Empty);
            AfterPropChangedEvt += AddressControllerList_AfterPropChangedEvt;
        }

        private void AddressControllerList_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(Search))) Filter = new(Search);
        }

 
        private void SelectedAddress(Address address) {
            Tenant tenant = WindowTracker.TenantForm?.Controller.Record;

            var oldAddress =tenant.TenantAddress;

            if (oldAddress!=null)
            {
                oldAddress.MovedOut = DateTime.Today;
                MainDB.TenantAddressTable.Update(oldAddress);
            }

            TenantAddress tenantAddress = new();
            tenantAddress.Address = address;
            tenantAddress.Tenant = tenant;            
            MainDB.TenantAddressTable.Insert(tenantAddress);             
        }

        #region SaveDeleteOpen

        protected override void OpenRecord(Address record)
        {
            throw new NotImplementedException();
        }

        protected override void SaveRecord(Address record)
        {
            SelectedRecord= record;
            if (!SelectedRecord.CanUpdate()) return;
            MainDB.AddressTable.Update(SelectedRecord);
        }

        protected override void DeleteRecord(Address record)
        {
            SelectedRecord = record;
            if (ConfirmDialog() == MessageBoxResult.Yes)
            {
                var index = RecordSource.IndexOf(SelectedRecord);
                MainDB.AddressTable.Delete(SelectedRecord);
                try
                {
                    SelectedRecord = RecordSource[index];
                }
                catch {
                    SelectedRecord = RecordSource[index-1];
                }
            }
        }

        protected override void OpenNewRecord()
        {
            AddressForm addressForm = new();
            addressForm.ShowDialog();
        }

        public override void SetRecordTrackerSource()
        {
        }

        #endregion
    }


    #region AddressFilterObject
    public class AddressFilter : IFilterList
    {
        public string? Search { get; set; }
        public IRecordSource Obj { get; set; }

        public AddressFilter(string? val)
        {
            Search = val;
            Obj = new Address();
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
