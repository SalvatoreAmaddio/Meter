using System;
using Testing.Controller;
using Testing.DB;
using Testing.Model;

namespace Meter.Controller
{
    public class PriceControllerForm : AbstractListDataController<Price>
    {

        public PriceControllerForm() {
            RecordSource.ReplaceRecords(MainDB.PriceTable.RecordSource);
        }

        protected override void OpenNewRecord()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteRecord(Price record)
        {
            throw new NotImplementedException();
        }

        protected override void OpenRecord(Price record)
        {
            throw new NotImplementedException();
        }

        protected override void SaveRecord(Price record)
        {
            throw new NotImplementedException();
        }

        public override void SetRecordTrackerSource()
        {
        }

    }
}
