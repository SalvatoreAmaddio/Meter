using Testing.DB;
using Testing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller;
using Meter.Utilities;

namespace Meter.Controller
{
    public class CityFormController : AbstractDataController<City>
    {       

        protected override void DeleteRecord(City record)
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

        protected override void SaveRecord(City record) {
            Record=record;
            var x = Record.CityName;
            Record.CityName = Sys.EachFirstLetterUpper(ref x);
            MainDB.CityTable.Insert(Record);
        }
}
}