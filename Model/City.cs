using Testing.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Model;
using Testing.RecordSource;
using Testing.Controller.Interfaces;
using Meter.Utilities;

namespace Testing.Model
{
    public class City : AbstractModel, IDB<City>, IRecordSource {

        private string _cityname=string.Empty;

        public Int64 CityID { get; set; }
        public string CityName { get => _cityname; set => Set<string>(ref value, ref _cityname, nameof(CityName)); }

        public City() {
            CityID= 0;
            CityName= string.Empty;
            IsDirty = false;
        }

        private void City_BeforePropChangedEvt(object? sender, Controller.NotifierArgs e)
        {
            if (e.PropIs(nameof(CityName)))
            {
                var val = e.GetValue<string>();
                val = Sys.EachFirstLetterUpper(ref val);
                e.SetValue<string>(val);
            }
        }


        public City(SQLiteDataReader reader) {
            CityID = reader.GetInt64(0);
            CityName = reader.GetString(1);
            BeforePropChangedEvt += City_BeforePropChangedEvt;
            IsDirty = false;
        }

        public string SQLSelect() => "SELECT * FROM City;";

        public string UpdateSelect() => "UPDATE City SET CityName=@CityName WHERE CityID=@CityID;";

        public string InsertSelect() => "INSERT INTO City (CityName) VALUES (@CityName);";

        public string DeleteSelect() => "DELETE FROM City WHERE CityID=@CityID;";

        public City GetRecord(SQLiteDataReader reader) => new(reader);

        public override bool Equals(object? obj)=>obj is City city && CityID == city.CityID;

        public override int GetHashCode()=>HashCode.Combine(CityID);
        

        public override string? ToString() => $"{CityName}";

        public string CompleteComboBoxSelectedItem()=>CityName;

        public bool FilterRecordSource(IFilterList filter)=>CityName.ToLower().Contains(filter.ToString().ToLower());
        
        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)
        {
            var source = (RecordSource<City>)ItemSource;
            source.ReplaceRecords(MainDB.CityTable.RecordSource);
            return source;
        }

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@CityID",CityID);
            param.AddWithValue("@CityName", CityName);
        }

        public override bool CanUpdate() =>!CityName.Equals(null);

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            throw new NotImplementedException();
        }

        public void SetLastInsertedID()=>CityID = MainDB.CityTable.LastInsertedID();
    }
}
