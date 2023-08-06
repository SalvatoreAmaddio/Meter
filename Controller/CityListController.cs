using Testing.DB;
using Testing.Model;
using Testing.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller;
using Testing.Controller.Interfaces;
using Testing.RecordSource;

namespace Meter.Controller {

    public class CityListController : AbstractListDataController<City> {

        CityFilter _filter = null!;
        public CityFilter Filter { get => _filter; set => Set<CityFilter>(ref value, ref _filter, nameof(Filter)); }

        public CityListController() {
            RecordSource.ReplaceRecords(MainDB.CityTable.RecordSource);
            SelectedRecord=RecordSource.FirstOrDefault();
            Filter = new(string.Empty);
            AfterPropChangedEvt += CityListController_AfterPropChangedEvt;
        }

        private void CityListController_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(Search))) Filter = new(Search);
        }

        protected override void OpenNewRecord()
        {
            CityForm cityForm = new ();
            cityForm.ShowDialog();
        }

        protected override void DeleteRecord(City record)
        {
            SelectedRecord= record;
            MainDB.CityTable.Delete(SelectedRecord);
        }

        protected override void OpenRecord(City record)
        {
            return;
        }

        protected override void SaveRecord(City record)
        {
            SelectedRecord= record;
            MainDB.CityTable.Update(SelectedRecord);
        }

        public override void SetRecordTrackerSource()
        {
        }

    }

    #region CityFilterObject
    public class CityFilter : IFilterList
    {
        public string? Search { get; set; }
        public IRecordSource Obj { get; set; }

        public CityFilter(string? val)
        {
            Search = val;
            Obj = new City();
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
