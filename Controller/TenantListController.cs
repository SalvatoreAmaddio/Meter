using Meter.Controller.Interfaces;
using Meter.View;
using System;
using System.Linq;
using System.Windows;
using Testing.Controller;
using Testing.Controller.Interfaces;
using Testing.DB;
using Testing.Model;
using Testing.RecordSource;

namespace Meter.Controller
{

    public class TenantListController : AbstractListDataController<Tenant> {

        TenantFilter _filter = null!;
        public TenantFilter Filter { get => _filter; set => Set(ref value, ref _filter, nameof(Filter)); }

        TenantOrder? _order;
        public TenantOrder? Order { get => _order; set => Set(ref value, ref _order, nameof(Order)); }

        public TenantListController()
        {
            Filter = new(string.Empty);
            Order = new();
            RecordSource.ReplaceRecords(MainDB.TenantTable.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
            AfterPropChangedEvt += TenantController_AfterPropChangedEvt;
        }

        public void OrderBy() =>Order = new();

        private void TenantController_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(Search))) Filter = new(e.GetValue<string?>());
            
        }

        public void Refresh()
        {
            RecordSource.ReplaceRecords(MainDB.TenantTable.RecordSource);
            SelectedRecord = RecordSource.FirstOrDefault();
            Order = new();
        }

        #region abstract methods implementation
        protected override void SaveRecord(Tenant record)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteRecord(Tenant record) {
            SelectedRecord = record;
            if (ConfirmDialog() == MessageBoxResult.Yes)
            {
                MainDB.TenantTable.Delete(SelectedRecord);
            }
        }

        protected override void OpenRecord(Tenant record)
        {
            SelectedRecord = record;
            TenantForm tenantForm = new(SelectedRecord);          
            tenantForm.Controller.SetRecordTracker(RecordSource);
            tenantForm.ShowDialog();
        }

        protected override void OpenNewRecord()
        {
            TenantForm tenantForm = new(new Tenant());
            tenantForm.ShowDialog();
        }

        public override void SetRecordTrackerSource()
        {
        }

        #endregion
    }

    #region TenantOrder
    public class TenantOrder : IListOrder
    {
        public IRecordSource Obj { get; set; } = new Tenant();
                
    }

    #endregion
    #region TenatFilterObject
    public class TenantFilter : IFilterList
    {
        public string? Search { get; set; }
        public IRecordSource Obj { get; set; }

        public TenantFilter(string? val)
        {
            Search = val;
            Obj = new Tenant();
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