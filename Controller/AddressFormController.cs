using Testing.DB;
using Testing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller;
using Testing.RecordSource;

namespace Testing.Controller {

    public class AddressFormController : AbstractDataController<Address> {

        public RecordSource<City> Cities { get; set; } = new();

        public AddressFormController() =>
        Cities.ReplaceRecords(MainDB.CityTable.RecordSource);
       
         protected override void SaveRecord(Address record) {
            Record = record;
            MainDB.AddressTable.Insert(Record);
         }

        protected override void DeleteRecord(Address record)
        {
            throw new NotImplementedException();
        }

        protected override void NextRecord()
        {
            throw new NotImplementedException();
        }

        protected override void PreviousRecord()
        {
            throw new NotImplementedException();
        }

        public override void SetRecordTrackerSource()
        {
        }

    }
}
