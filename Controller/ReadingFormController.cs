using Testing.DB;
using Testing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller;

namespace Meter.Controller
{
    public class ReadingFormController : AbstractDataController<Reading> {

        public ReadingFormController() { }

        protected override void DeleteRecord(Reading record)
        {
            throw new NotImplementedException();
        }

        public override void SetRecordTrackerSource()
        {
        }

        protected override void SaveRecord(Reading record) {
            MainDB.ReadingTable.Record = record;
            MainDB.ReadingTable.Insert();
            MainDB.ReadingTable.Select();
            WindowTracker.TenantForm.Controller.AddressTenantControllerList.ReadingListController.RecordSource.ReplaceRecords(MainDB.ReadingTable.RecordSource);

            ReadingFilter filter = new()
            {
                TenantAddressID = record.TenantAddress.TenantAddressID,
                IgnoreDate = WindowTracker.TenantForm.Controller.AddressTenantControllerList.ReadingListController.Filter.IgnoreDate,
            };

            WindowTracker.TenantForm.Controller.AddressTenantControllerList.ReadingListController.Filter = filter;
        }
    }
}
